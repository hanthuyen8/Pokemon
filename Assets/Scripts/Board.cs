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

        private Block[,] _blockArrays;

        private void Awake()
        {
            _blockArrays = new Block[_maxCol, Mathf.CeilToInt(_allBlocks.Count / _maxCol)];
            int col = 0, row = 0;
            for (int i = 0; i < _allBlocks.Count; i++)
            {
                if (col == _maxCol)
                {
                    col = 0;
                    row++;
                }

                _blockArrays[col, row] = _allBlocks[i];
                col++;
            }
        }

        private void Start()
        {
            SpawnNewItemIntoBlocks(_allBlocks);
        }

        public void SpawnNewItemIntoBlocks(List<Block> emptyBlocks)
        {
            for (int i = 0; i < emptyBlocks.Count; i++)
            {
                Item item = AddNewItem();
                item.MoveToBlock(emptyBlocks[i]);
            }
        }

        public void FillTheEmptyBlocks(Queue<Block> emptyBlocks)
        {
            // List nhập vào phải được sắp xếp theo thứ tự rồi.
            while(emptyBlocks.Count > 0)
            {
                var block = emptyBlocks.Dequeue();
                var upperRow = block.AtRow - 1;
                bool added = false;
                while (upperRow >= 0)
                {
                    var upperBlock = _blockArrays[block.AtColumn, upperRow];
                    if (!upperBlock.IsEmpty)
                    {
                        // Tìm thấy block không trống.
                        // Kéo nó xuống block hiện tại.
                        upperBlock.HasItem.MoveToBlock(block);
                        upperBlock.RemoveItem();
                        emptyBlocks.Enqueue(upperBlock);
                        added = true;
                        break;
                    }
                    else
                    {
                        upperRow--;
                        continue;
                    }
                }
                if(!added)
                {
                    // Không còn block nào phía trên đầu nữa. Tạo Item mới.
                    Item item = AddNewItem();
                    item.MoveToBlock(block);
                }
            }
        }

        private Item AddNewItem()
        {
            Item item = Instantiate(_itemPrefab);
            item.Type = _itemTypes[Random.Range(0, _itemTypes.Count - 1)];
            item.transform.SetParent(transform);
            _inGameItems.Add(item);
            return item;
        }

        [Button("Regenerate Board")]
        private void RegenerateBoard()
        {
            int col = 0, row = 0;
            _allBlocks = new List<Block>(GetComponentsInChildren<Block>());

            foreach (var block in _allBlocks)
            {
                block.Init(col, row);
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