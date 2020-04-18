using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Item : MonoBehaviour
    {
        public static event System.Action<Item> PlayerWantToChooseThisItem;
        public static event System.Action PlayerConfirm;

        public ItemType Type;
        public Block InsideThisBlock { get; private set; }

        [SerializeField] private SpriteRenderer _avatar = null;
        [SerializeField] private SpriteRenderer _shadow = null;

        private static bool _isMouseDown = false;
        private bool _isChosen = false;


        private void Awake()
        {
            Assert.IsNotNull(_avatar);
            Assert.IsNotNull(_shadow);

            PlayerWantToChooseThisItem += Item_PlayerWantToChooseThisItem;
            _shadow.enabled = false;
        }

        private void Start()
        {
            _avatar.sprite = Type.Img;
        }

        private void OnDestroy()
        {
            PlayerWantToChooseThisItem -= Item_PlayerWantToChooseThisItem;
        }

        public static void ReleaseAll()
        {
            _isMouseDown = false;
        }

        public void MoveToBlock(Block block)
        {
            var blockPos = block.transform.localPosition;
            if (InsideThisBlock == null)
            {
                transform.localPosition = new Vector3(blockPos.x, 20, -1);
            }

            InsideThisBlock = block;
            InsideThisBlock.KeepThisItem(this);

            transform.DOLocalMoveY(blockPos.y, 1f);
        }

        public bool LevelUp()
        {
            if (Type.NextLevel)
            {
                Type = Type.NextLevel;
                _avatar.sprite = Type.Img;
                return true;
            }
            return false;
        }

        public void ChooseThisItem()
        {
            _isChosen = true;
            _shadow.enabled = true;
        }

        public void UnChooseThisItem()
        {
            _isChosen = false;
            _shadow.enabled = false;
        }

        public void RemoveThisItem()
        {
            InsideThisBlock.RemoveItem();
            Destroy(this);
        }

        #region Flashing
        private Tween _flashing;

        public void Flash()
        {
            _shadow.enabled = true;
            _flashing = _shadow.DOFade(0.5f, 0.3f).SetLoops(8, LoopType.Yoyo).OnComplete(() => _shadow.enabled = false);
        }

        private void Item_PlayerWantToChooseThisItem(Item current)
        {
            _flashing?.Kill();
            _shadow.enabled = _isChosen;
            var color = _shadow.color;
            color.a = 1;
            _shadow.color = color;
        }
        #endregion

        #region MouseInput
        private void OnMouseDown()
        {
            _isMouseDown = true;
            _isChosen = true;

            PlayerWantToChooseThisItem?.Invoke(this);
        }

        private void OnMouseOver()
        {
            if (!_isMouseDown)
                return;

            if (_isChosen)
                return;

            _isChosen = true;
            PlayerWantToChooseThisItem?.Invoke(this);
        }

        private void OnMouseUp()
        {
            _isMouseDown = false;
            PlayerConfirm?.Invoke();
        }
        #endregion

    }

}