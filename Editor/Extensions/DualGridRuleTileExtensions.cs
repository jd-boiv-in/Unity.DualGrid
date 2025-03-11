using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace skner.DualGrid.Editor.Extensions
{
    public static class DualGridRuleTileExtensions
    {

        /// <summary>
        /// Applies the provided <paramref name="texture"/> to the <paramref name="dualGridRuleTile"/>.
        /// <para></para>
        /// If the texture is split in 16x sprites, an automatic rule tiling prompt will follow.
        /// <para></para>
        /// Otherwise, the texture is incompatible and will not be applied, displaying a warning popup.
        /// </summary>
        /// <param name="dualGridRuleTile"></param>
        /// <param name="texture"></param>
        /// <param name="ignoreAutoSlicePrompt"></param>
        /// <returns><see langword="true"/> if the texture was applied, <see langword="false"/> otherwise.</returns>
        public static bool TryApplyTexture2D(this DualGridRuleTile dualGridRuleTile, Texture2D texture, bool ignoreAutoSlicePrompt = false)
        {
            var list = texture.GetSplitSpritesFromTexture();
            if (list.Count == 1) // Assume we need to slice the texture
            {
                // Manually slice it by code
                var texturePath = AssetDatabase.GetAssetPath(texture);
                var textureImporter = (TextureImporter) AssetImporter.GetAtPath(texturePath);
                if (textureImporter == null)
                    throw new Exception($"Cannot not find TextureImporter");

                textureImporter.spritePixelsPerUnit = 64; // TODO: Add as a setting somewhere...
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                
                var importerSettings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(importerSettings);
                importerSettings.spriteGenerateFallbackPhysicsShape = false;
                textureImporter.SetTextureSettings(importerSettings);
                
                // Slice to 16 pieces
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
                dataProvider.InitSpriteEditorDataProvider();
                
                var colCount = 4;
                var rowCount = 4;
                var spriteSize = texture.width / colCount;

                var n = 0;
                var metas = new List<SpriteRect>();
                for (var y = rowCount - 1; y >= 0; y--)
                for (var x = 0; x < colCount; x++)
                {
                    var meta = new SpriteRect();
                    meta.rect = new Rect(x * spriteSize, y * spriteSize, spriteSize, spriteSize);
                    meta.name = $"{texture}_{n++}";
                    metas.Add(meta);
                }
                
                dataProvider.SetSpriteRects(metas.ToArray());
                dataProvider.Apply();
                
                AssetDatabase.ImportAsset(texturePath);
                list = texture.GetSplitSpritesFromTexture();
            }
            
            List<Sprite> sprites = list.OrderBy(sprite =>
            {
                var exception = new InvalidOperationException($"Cannot perform automatic tiling because sprite name '{sprite.name}' is not standardized. It must end with a '_' and a number. Example: 'tile_9'");

                var spriteNumberString = sprite.name.Split("_").LastOrDefault() ?? throw exception;
                bool wasParseSuccessful = int.TryParse(spriteNumberString, out int spriteNumber);

                if (wasParseSuccessful) return spriteNumber;
                else throw exception;
            }).ToList();

            bool isTextureSlicedIn16Pieces = sprites.Count == 16;

            if (isTextureSlicedIn16Pieces)
            {
                bool shouldAutoSlice = ignoreAutoSlicePrompt || EditorUtility.DisplayDialog("16x Sliced Texture Detected",
                    "The selected texture is sliced in 16 pieces. Perform automatic rule tiling?", "Yes", "No");

                dualGridRuleTile.OriginalTexture = texture;
                ApplySprites(ref dualGridRuleTile, sprites);

                if (shouldAutoSlice)
                    AutoDualGridRuleTileProvider.ApplyConfigurationPreset(ref dualGridRuleTile);

                return true;
            }
            else
            {
                EditorUtility.DisplayDialog($"{dualGridRuleTile.name} - Incompatible Texture Detected", "The selected texture is not sliced in 16 pieces.\nTexture will not be applied.", "Ok");
                return false;
            }
        }

        private static void ApplySprites(ref DualGridRuleTile dualGridRuleTile, List<Sprite> sprites)
        {
            dualGridRuleTile.m_DefaultSprite = sprites.FirstOrDefault();
            if (6 < sprites.Count) dualGridRuleTile.m_DefaultSprite = sprites[6];
            
            dualGridRuleTile.m_TilingRules.Clear();

            foreach (Sprite sprite in sprites)
            {
                AddNewTilingRuleFromSprite(ref dualGridRuleTile, sprite);
            }
        }

        private static void AddNewTilingRuleFromSprite(ref DualGridRuleTile tile, Sprite sprite)
        {
            tile.m_TilingRules.Add(new DualGridRuleTile.TilingRule() { m_Sprites = new Sprite[] { sprite }, m_ColliderType = UnityEngine.Tilemaps.Tile.ColliderType.None });
        }

        /// <summary>
        /// Returns a sorted list of <see cref="Sprite"/>s from a provided <paramref name="texture"/>.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static List<Sprite> GetSplitSpritesFromTexture(this Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToList();
        }

    }
}
