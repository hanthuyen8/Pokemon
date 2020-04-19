using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Game
{
    public class ScoreCalculator : MonoBehaviour
    {
        // Cách tính điểm;
        /*
        - Kéo 3 con lv1: 10 điểm
        - Kéo 4 con lv1: 20 điểm
        - Kéo 5 con lv2: 30 điểm và cứ thế tính lên
        Bonus:
        - Chọn đúng liên tục: x với số lần liên tục
             */

        public static ScoreCalculator Instance { get; private set; }
        private static readonly object _lock = new object();

        public int BonusCount { get; private set; } = 0;

        [SerializeField] private Text ScoreLabel = null;
        private int _totalScore = 0;

        private void Awake()
        {
            lock(_lock)
            {
                if( Instance == null)
                {
                    Instance = this;
                }
                else
                {
                    Destroy(this);
                    return;
                }
            }
        }

        public void AddScore(int itemCount)
        {
            BonusCount++;
            _totalScore += (itemCount - 2) * 10 * BonusCount;

            ScoreLabel.text = _totalScore.ToString();
        }

        public void LoseBonus()
        {
            BonusCount = 0;
        }
    }

}