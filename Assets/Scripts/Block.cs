using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    public class Block : MonoBehaviour
    {
        public static event System.Action ABlockReady;
        public int BlockValue => AtColumn + AtRow;

        public int AtColumn => _atColumn;
        public int AtRow => _atRow;
        public Item HasItem { get; private set; }
        public bool IsEmpty => HasItem == null;
        public List<Block> Neighbours { get; } = new List<Block>();

        public bool IsForbidden = true;

        [SerializeField] private int _atColumn;
        [SerializeField] private int _atRow;

        public void Init(int atCol, int atRow)
        {
            _atColumn = atCol;
            _atRow = atRow;
            IsForbidden = false;
        }

        private void Start()
        {
            FindNeighbours();
            ABlockReady?.Invoke();
        }

        public void KeepThisItem(Item item)
        {
            HasItem = item;
        }

        public void RemoveItem()
        {
            HasItem = null;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (other is Block block)
            {
                return block == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Block block1, Block block2)
        {
            if (block1 is null)
                return block2 is null;

            if (block2 is null)
                return false;

            return block1.AtColumn == block2.AtColumn && block1.AtRow == block2.AtRow;
        }

        public static bool operator !=(Block block1, Block block2)
        {
            return !(block1 == block2);
        }

        private void FindNeighbours()
        {
            Board board = Board.Instance;
            Block neighbour;

            if(board.IsThisBlockExist(AtColumn - 1, AtRow, out neighbour))
            {
                // left
                Neighbours.Add(neighbour);
            }
            if(board.IsThisBlockExist(AtColumn + 1, AtRow, out neighbour))
            {
                // right
                Neighbours.Add(neighbour);
            }
            if (board.IsThisBlockExist(AtColumn, AtRow - 1, out neighbour))
            {
                // top
                Neighbours.Add(neighbour);
            }
            if (board.IsThisBlockExist(AtColumn, AtRow + 1, out neighbour))
            {
                // bottom
                Neighbours.Add(neighbour);
            }
        }
    }
}