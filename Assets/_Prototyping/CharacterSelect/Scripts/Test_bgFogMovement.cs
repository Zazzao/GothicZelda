using UnityEngine;


namespace DensetsuEngine.Prototyping
{
    public class Test_bgFogMovement : MonoBehaviour
    {

        public Vector2 targetPosition;
        public float speed = 5f;


        void Start()
        {

        }

        
        void Update()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            // Smoothly step towards the target position
            rectTransform.anchoredPosition = Vector2.MoveTowards(
                rectTransform.anchoredPosition,
                targetPosition,
                speed * Time.deltaTime
            );


            if (rectTransform.anchoredPosition.x > 600) {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - 1536, rectTransform.anchoredPosition.y);
            }  
        }
    }
}
