using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class DragAndDrop
    {
        private DndState state;
        private GameObject item;
        private Vector2 offset;
        public event EventHandler PickedUp;
        public event EventHandler DropedDown;

        public Vector2 FromPos { get; private set; }
        public Vector2 ToPos { get; private set; }

        public DragAndDrop()
        {
            state = DndState.None;
            
        }

        public bool Action()
        {
            switch (state)
            {
                case DndState.None:
                    {
                        if (IsMouseButtonPressed())
                        {
                            PickUp();
                        }
                        break;
                    }
                case DndState.Drag:
                    {
                        if(IsMouseButtonPressed())
                        {
                            Drag();
                        }
                        else
                        {
                            Drop();
                            return true;
                        }
                        break;
                    }
                default:
                    break;
            }

            return false;
        }

        private bool IsMouseButtonPressed()
        {
            return Input.GetMouseButton(0);
        }

        private void PickUp()
        {
            var clickPosition = GetClickPosition();
            var clickedItem = GetItemAt(clickPosition);

            if (clickedItem == null)
                return;
            
            FromPos = clickedItem.position;
           if (PickedUp != null)
               PickedUp.Invoke(this, null);

            item = clickedItem.gameObject;
            state = DndState.Drag;
            offset = FromPos - clickPosition;
        }

        private Vector2 GetClickPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private Transform GetItemAt(Vector2 position)
        {
            var figures = Physics2D.RaycastAll(position, position, 0.5f);
            if (figures.Length == 0)
                return null;
            else
                return figures[0].transform;
        }

        private void Drag()
        {
            item.transform.position = GetClickPosition() + offset;
        }

        private void Drop()
        {
            ToPos = item.transform.position;
            state = DndState.None;
            item = null;
            if (DropedDown != null)
                DropedDown.Invoke(this, null);
        }

        enum DndState
        {
            None,
            Drag
        }
    }
}
