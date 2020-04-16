using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private int _maxCol = 5;
        [SerializeField] private float _blockSize = 1;

        [Space]

        [SerializeField] private List<ItemType> _itemTypes = null;
        [SerializeField] private Item _itemPrefab = null;

        [Space]

        [SerializeField] private List<Block> _allBlocks = null;
        [SerializeField] private List<Item> _inGameItems = null;

        private void Start()
        {
            SpawnNewItemIntoBlocks(_allBlocks);
        }

        public void SpawnNewItemIntoBlocks(List<Block> emptyBlocks)
        {
            for (int i = 0; i < emptyBlocks.Count; i++)
            {
                Item item = Instantiate(_itemPrefab);
                item.Type = _itemTypes[Random.Range(0, _itemTypes.Count - 1)];
                item.InsideThisBlock = emptyBlocks[i];
                item.transform.SetParent(transform);

                _inGameItems.Add(item);
            }
        }

        public void RearrangeBoard()
        {
            // Sắp xếp lại Board: tìm ô trống và kéo item của block trên đó rơi xuống.
        }

        [Button("Regenerate Board")]
        private void RegenerateBoard()
        {
            int col = 0, row = 0;
            _allBlocks = new List<Block>(GetComponentsInChildren<Block>());

            foreach (var block in _allBlocks)
            {
                block.BlockId = new Vector2(col, row);
                block.name = $"Block {col} - {row}";
                block.transform.localPosition = new Vector3(_blockSize * col, -_blockSize * row, 0);

                if (++col >= _maxCol)
                {
                    col = 0;
                    row++;
                }
            }
        }
        
    }

}