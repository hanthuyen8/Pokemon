using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public Board board;
        public float PlayTime { get; private set; }
        public float LastTimePlayerInterract { get; private set; }
        private readonly List<Item> PlayerChosenItems = new List<Item>();

        private void Awake()
        {
            Assert.IsNotNull(board);

            Item.PlayerWantToChooseThisItem += Item_PlayerWantToChooseThisItem;
            Item.PlayerConfirm += Item_PlayerConfirm;
        }

        private void Start()
        {
            PrepareNewHint();
        }

        private void Update()
        {
            PlayTime += Time.deltaTime;
            if(PlayTime - LastTimePlayerInterract >= GameSettings.MIN_AMOUNT_OF_TIME_TO_SHOW_HINT)
            {
                ShowHint();
            }
        }

        private void OnDestroy()
        {
            Item.PlayerWantToChooseThisItem -= Item_PlayerWantToChooseThisItem;
            Item.PlayerConfirm -= Item_PlayerConfirm;
        }

        private void Item_PlayerWantToChooseThisItem(Item item)
        {
            LastTimePlayerInterract = PlayTime;

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
                item.UnChooseThisItem();
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
            if (PlayerChosenItems.Count < GameSettings.MIN_AMOUNT_OF_BLOCKS_TO_COMBINE)
            {
                PlayerChosenItems.ForEach(x => x.UnChooseThisItem());
                ClearList();
                return;
            }

            var lastIndex = PlayerChosenItems.Count - 1;
            if (lastIndex < 0)
                return;

            var lastItem = PlayerChosenItems[lastIndex];
            lastItem.UnChooseThisItem();
            if (lastItem.LevelUp())
            {
                PlayerChosenItems.RemoveAt(lastIndex);
            }

            var emptyBlocks = PlayerChosenItems.ConvertAll(x => x.InsideThisBlock);

            PlayerChosenItems.ForEach(x => Destroy(x.gameObject));
            emptyBlocks.ForEach(x => x.RemoveItem());
            ClearList();
            SendSortedBlocksToSpawnNewItems(emptyBlocks);
            PrepareNewHint();

            void ClearList()
            {
                PlayerChosenItems.Clear();
                Item.ReleaseAll();
            }
        }

        private bool IsTheSameId(Item item1, Item item2)
        {
            return item1.Type.Id == item2.Type.Id;
        }

        private static bool IsItemsNearBy(Item item1, Item item2)
        {
            var block1 = item1.InsideThisBlock;
            var block2 = item2.InsideThisBlock;

            if (block1.AtColumn == block2.AtColumn)
                return Mathf.Abs(block1.AtRow - block2.AtRow) == 1;

            if (block1.AtRow == block2.AtRow)
                return Mathf.Abs(block1.AtColumn - block2.AtColumn) == 1;

            return false;
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

        #region Hints
        private List<Block> _hint;

        private async void PrepareNewHint()
        {
            _hint = await Task.Run(() => Hints.FindAHint(Board.Instance.GetArray()));
        }

        private void ShowHint()
        {
            if (_hint != null)
            {
                _hint.ForEach(x => x.HasItem.Flash());
                LastTimePlayerInterract = PlayTime;
            }
        }
        #endregion
    }

}