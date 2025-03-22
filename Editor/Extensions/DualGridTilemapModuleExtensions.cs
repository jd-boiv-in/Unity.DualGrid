using System.Collections.Generic;
using skner.DualGrid.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace skner.DualGrid.Editor.Extensions
{
    public static class DualGridTilemapModuleExtensions
    {
        private struct PreviewTileTemp
        {
            public Vector3Int Position;
            public bool HadTile;
        }
        
        private static List<PreviewTileTemp> _previewTilesA = new List<PreviewTileTemp>(50);
        private static List<PreviewTileTemp> _previewTilesB = new List<PreviewTileTemp>(50);
        
        public static void SetEditorPreviewTile(this DualGridTilemapModule dualGridTilemapModule, Vector3Int position, TileBase tile)
        {
            dualGridTilemapModule.DataTilemap.SetEditorPreviewTile(position, tile);
            dualGridTilemapModule.UpdatePreviewRenderTiles(position);
        }

        public static void ClearEditorPreviewTile(this DualGridTilemapModule dualGridTilemapModule, Vector3Int position)
        {
            dualGridTilemapModule.DataTilemap.SetEditorPreviewTile(position, null);
            dualGridTilemapModule.UpdatePreviewRenderTiles(position);
        }

        public static void UpdatePreviewRenderTiles(this DualGridTilemapModule dualGridTilemapModule, Vector3Int previewDataTilePosition)
        {
            bool hasPreviewDataTile = dualGridTilemapModule.DataTilemap.HasEditorPreviewTile(previewDataTilePosition);
            bool isPreviewDataTileVisible = dualGridTilemapModule.DataTilemap.GetEditorPreviewTile<DualGridPreviewTile>(previewDataTilePosition) is DualGridPreviewTile previewTile && previewTile.IsFilled;

            _previewTilesB.Clear();
            foreach (Vector3Int renderTilePosition in DualGridUtils.GetRenderTilePositions(previewDataTilePosition))
            {
                if (hasPreviewDataTile && isPreviewDataTileVisible)
                    SetPreviewRenderTile(dualGridTilemapModule, renderTilePosition);
                else
                    UnsetPreviewRenderTile(dualGridTilemapModule, renderTilePosition);
            }

            (_previewTilesB, _previewTilesA) = (_previewTilesA, _previewTilesB);
        }

        public static void UpdateAllPreviewRenderTiles(this DualGridTilemapModule dualGridTilemapModule)
        {
            foreach (var position in dualGridTilemapModule.DataTilemap.cellBounds.allPositionsWithin)
            {
                dualGridTilemapModule.UpdatePreviewRenderTiles(position);
            }
        }

        public static void ClearAllPreviewTiles(this DualGridTilemapModule dualGridTilemapModule)
        {
            dualGridTilemapModule.DataTilemap.ClearAllEditorPreviewTiles();
            dualGridTilemapModule.RenderTilemap.ClearAllEditorPreviewTiles();
        }

        private static void SetPreviewRenderTile(DualGridTilemapModule dualGridTilemapModule, Vector3Int previewRenderTilePosition)
        {
            _previewTilesB.Add(new PreviewTileTemp()
            {
                Position = previewRenderTilePosition,
                HadTile = dualGridTilemapModule.RenderTilemap.HasTile(previewRenderTilePosition)
            });
            
            dualGridTilemapModule.RenderTilemap.SetEditorPreviewTile(previewRenderTilePosition, dualGridTilemapModule.RenderTile);
            dualGridTilemapModule.RenderTilemap.SetTile(previewRenderTilePosition, dualGridTilemapModule.RenderTile);
            
            // Old
            //dualGridTilemapModule.RenderTilemap.SetEditorPreviewTile(previewRenderTilePosition, dualGridTilemapModule.RenderTile);
            //dualGridTilemapModule.RenderTilemap.RefreshTile(previewRenderTilePosition);
        }

        private static void UnsetPreviewRenderTile(DualGridTilemapModule dualGridTilemapModule, Vector3Int previewRenderTilePosition)
        {
            dualGridTilemapModule.RenderTilemap.SetEditorPreviewTile(previewRenderTilePosition, null);

            foreach (var tile in _previewTilesA)
            {
                if (tile.Position != previewRenderTilePosition) continue;
                if (tile.HadTile)
                    dualGridTilemapModule.RenderTilemap.SetTile(previewRenderTilePosition, dualGridTilemapModule.RenderTile);
                else
                    dualGridTilemapModule.RenderTilemap.SetTile(previewRenderTilePosition, null);
                return;
            }
            
            // Not found, might've been just added
            if (dualGridTilemapModule.RenderTilemap.HasTile(previewRenderTilePosition))
                dualGridTilemapModule.RenderTilemap.SetTile(previewRenderTilePosition, dualGridTilemapModule.RenderTile);
            else
                dualGridTilemapModule.RenderTilemap.SetTile(previewRenderTilePosition, null);

            // Old
            //dualGridTilemapModule.RenderTilemap.SetEditorPreviewTile(previewRenderTilePosition, null);
            //dualGridTilemapModule.RenderTilemap.RefreshTile(previewRenderTilePosition);
        }
    }
}
