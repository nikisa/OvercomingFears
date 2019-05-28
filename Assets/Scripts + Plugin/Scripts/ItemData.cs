using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class ItemData {
        public enum Type {Node , Enemy , Obstacle , Player , MovableObject , Floor}

        public Vector3 BoardPosition;
        public Type ItemType;
    }