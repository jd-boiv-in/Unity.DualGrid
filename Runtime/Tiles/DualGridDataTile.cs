using UnityEngine;
using UnityEngine.Tilemaps;

namespace skner.DualGrid
{
    // Same as tile but without the `GetData` override
    public class DualGridDataTile : TileBase
    {
        [SerializeField]
        private Sprite m_Sprite;
        [SerializeField]
        private Color m_Color = Color.white;
        [SerializeField]
        private Matrix4x4 m_Transform = Matrix4x4.identity;
        [SerializeField]
        private GameObject m_InstancedGameObject;
        [SerializeField]
        private TileFlags m_Flags = TileFlags.LockColor;
        [SerializeField]
        private Tile.ColliderType m_ColliderType = Tile.ColliderType.Sprite;
    
        /// <summary>
        ///   <para>Sprite to be rendered at the Tile.</para>
        /// </summary>
        public Sprite sprite
        {
          get => this.m_Sprite;
          set => this.m_Sprite = value;
        }
    
        /// <summary>
        ///   <para>Color of the Tile.</para>
        /// </summary>
        public Color color
        {
          get => this.m_Color;
          set => this.m_Color = value;
        }
    
        /// <summary>
        ///   <para>Matrix4x4|Transform matrix of the Tile.</para>
        /// </summary>
        public Matrix4x4 transform
        {
          get => this.m_Transform;
          set => this.m_Transform = value;
        }
    
        /// <summary>
        ///   <para>GameObject of the Tile.</para>
        /// </summary>
        public GameObject gameObject
        {
          get => this.m_InstancedGameObject;
          set => this.m_InstancedGameObject = value;
        }
    
        /// <summary>
        ///   <para>TileFlags of the Tile.</para>
        /// </summary>
        public TileFlags flags
        {
          get => this.m_Flags;
          set => this.m_Flags = value;
        }
    
        public Tile.ColliderType colliderType
        {
          get => this.m_ColliderType;
          set => this.m_ColliderType = value;
        }
    }
    
    /*public class DualGridDataTile : Tile
    {

        private DualGridTilemapModule _dualGridTilemapModule;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            SetDataTilemap(tilemap);

            base.GetTileData(position, tilemap, ref tileData);

            // Sets the tile data's GameObject based on the associated DualGridTilemapModule's setting
            if (_dualGridTilemapModule != null && _dualGridTilemapModule.GameObjectOrigin != GameObjectOrigin.DataTilemap)
            {
                tileData.gameObject = null;
            }
        }

        private void SetDataTilemap(ITilemap tilemap)
        {
            var originTilemap = tilemap.GetComponent<Tilemap>();
            _dualGridTilemapModule = originTilemap.GetComponent<DualGridTilemapModule>();
        }

    }*/
}
