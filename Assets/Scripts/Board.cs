using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    public class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }

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
            if(!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            Block.ABlockReady += Block_ABlockReady;

            _blockArrays = new Block[_maxCol, Mathf.CeilToInt(_allBlocks.Count / (float)_maxCol)];
            int col = 0, row = 0;
            for (int i = 0; i < _allBlocks.Count; i++)
            {
                if (col == _maxCol)
                {
                    col = 0;
                    row++;
                }

                if (i >= _allBlocks.Count)
                    break;

                _blockArrays[col, row] = _allBlocks[i];
                col++;
            }
        }

        private void Start()
        {
            SpawnNewItemIntoBlocks(_allBlocks);
        }

        private void OnDestroy()
        {
            Block.ABlockReady -= Block_ABlockReady;
        }

        public bool IsThisBlockExist(int atColumn, int atRow, out Block result)
        {
            if (atColumn < 0 || atRow < 0 || 
                atColumn >= _blockArrays.GetLength(0) || atRow >= _blockArrays.GetLength(1))
            {
                result = null;
                return false;
            }

            result = _blockArrays[atColumn, atRow];
            return result != null;
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
            while (emptyBlocks.Count > 0)
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
                if (!added)
                {
                    // Không còn block nào phía trên đầu nữa. Tạo Item mới.
                    Item item = AddNewItem();
                    item.MoveToBlock(block);
                }
            }
        }

        public Block[,] GetArray()
        {
            return _blockArrays;
        }

        private Item AddNewItem()
        {
            Item item = Instantiate(_itemPrefab);
            item.Type = _itemTypes[Random.Range(0, _itemTypes.Count - 1)];
            item.transform.SetParent(transform);
            _inGameItems.Add(item);
            return item;
        }

        private int _blockReadyCount = 0;
        private void Block_ABlockReady()
        {
            /*if (++_blockReadyCount == _allBlocks.Count)
            {
                var hint = Hints.FindAHint(_blockArrays);
                hint.ForEach(x => x.HasItem.ChooseThisItem());
            }*/
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