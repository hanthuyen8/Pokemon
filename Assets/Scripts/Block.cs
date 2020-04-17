using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    public class Block : MonoBehaviour
    {
        public int BlockValue => AtColumn + AtRow;

        public int AtColumn => _atColumn;
        public int AtRow => _atRow;
        public Item HasItem { get; private set; }
        public bool IsEmpty => HasItem == null;

        public bool IsForbidden = true;

        [SerializeField] private int _atColumn;
        [SerializeField] private int _atRow;

        public void Init(int atCol, int atRow)
        {
            _atColumn = atCol;
            _atRow = atRow;
            IsForbidden = false;
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
    }
}