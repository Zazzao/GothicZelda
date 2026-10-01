using UnityEngine;

namespace DensetsuEngine.Prototyping
{
    public interface IMenuNavigation
    {
        public void LoadMenu();

        public void UnloadMenu();
        public void OnNavUp();
        public void OnNavDown();
        public void OnNavLeft();
        public void OnNavRight();
        public void OnNavSelect();
        public void OnNavCancel();

    }
}
