using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public Board board;
        private List<Item> PlayerChosenItems = new List<Item>();

        private void Awake()
        {
            Assert.IsNotNull(board);

            Item.PlayerChooseThisItem += Item_PlayerChooseThisItem;
            Item.PlayerConfirm += Item_PlayerConfirm;
        }

        private void OnDestroy()
        {
            Item.PlayerChooseThisItem -= Item_PlayerChooseThisItem;
            Item.PlayerConfirm -= Item_PlayerConfirm;
        }

        private void Item_PlayerChooseThisItem(Item item)
        {
            int count = PlayerChosenItems.Count;

            if (count == 0)
            {
                PlayerChooseRight();
                return;
            }

            Item lastItem = PlayerChosenItems[count - 1];

            // Sẽ kiểm tra 2 thứ:
            // 1. Là Id phải trùng nhau
            // 2. Là tọa độ phải gần nhau (tổng chênh lệch x+y không lớn hơn 1)

            if (!IsTheSameId(item, lastItem))
            {
                PlayerChooseWrong();
                return;
            }

            if (!IsItemsNearBy(item, lastItem))
            {
                PlayerChooseWrong();
                return;
            }

            PlayerChooseRight();

            void PlayerChooseWrong()
            {
                Item.ReleaseAll();
                PlayerChosenItems.ForEach(x => x.UnChooseThisItem());
                PlayerChosenItems.Clear();
            }

            void PlayerChooseRight()
            {
                PlayerChosenItems.Add(item);
                item.ChooseThisItem();
            }
        }

        private void Item_PlayerConfirm()
        {
            // Gộp tất cả lại thành 1, xóa hết list và tạo ra 1 item mới ngay vị trí của phần tử cuối cùng.
            var lastIndex = PlayerChosenItems.Count - 1;
            if (lastIndex < 0)
                return;

            var lastItem = PlayerChosenItems[lastIndex];
            lastItem.UnChooseThisItem();

            PlayerChosenItems.RemoveAt(lastIndex);
            var emptyBlocks = PlayerChosenItems.ConvertAll(x => x.InsideThisBlock);

            PlayerChosenItems.ForEach(x => Destroy(x.gameObject));
            PlayerChosenItems.Clear();

            Item.ReleaseAll();

            SendSortedBlocksToSpawnNewItems(emptyBlocks);
        }

        private bool IsTheSameId(Item item1, Item item2)
        {
            return item1.Type.Id == item2.Type.Id;
        }

        private bool IsItemsNearBy(Item item1, Item item2)
        {
            int distance = Mathf.Abs(item1.InsideThisBlock.BlockValue - item2.InsideThisBlock.BlockValue);

            if (distance > 1)
                return false;

            return true;
        }

        private void SendSortedBlocksToSpawnNewItems(List<Block> blocks)
        {
            var columns = blocks.GroupBy(x => x.AtColumn);

            foreach(var group in columns)
            {
                var sorted = group.OrderByDescending(x => x.AtRow);
                Queue<Block> queue = new Queue<Block>();
                foreach(var item in sorted)
                {
                    queue.Enqueue(item);
                }
                board.FillTheEmptyBlocks(queue);
            }
        }
    }

}