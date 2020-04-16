using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Game/New Item Type")]
    public class ItemType : ScriptableObject
    {
        public string Id;
        public Sprite Img;

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(Id))
                Id = this.name;
        }
    }

}