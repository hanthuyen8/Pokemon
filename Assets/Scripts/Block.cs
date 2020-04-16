using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    public class Block : MonoBehaviour
    {
        public int BlockValue => (int)(BlockId.x + BlockId.y);

        public Vector2 BlockId;
        public bool IsEmpty = false;
    }
}