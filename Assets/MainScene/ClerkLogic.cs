using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClerkLogic : MonoBehaviour{
    public Transform player;
    public float TriggerDis = 3.5f;
    public float Distance;
    public Button InteracctPrefab;
    public Button InteractButton;
    public Canvas canva;
    void Start(){}

    void Update(){
        Distance = 
        (player.position.x - gameObject.transform.position.x) * (player.position.x - gameObject.transform.position.x) +
        (player.position.y - gameObject.transform.position.y) * (player.position.y - gameObject.transform.position.y);
        if(InteractButton != null && Distance >= TriggerDis){
            InteractButton.gameObject.SetActive(false);
            Destroy(InteractButton.gameObject);
        }
        if(InteractButton == null && Distance <= TriggerDis){
            InteractButton = Instantiate(InteracctPrefab,canva.transform);
            InteractButton.gameObject.SetActive(true);
            if(InteractButton.TryGetComponent<RectTransform>(out RectTransform rect)){
                rect.anchoredPosition = new Vector2(45,25);
            }
            InteractButton.onClick.AddListener(OnButtonClick);
            UpdateText();
        }
        if (Keyboard.current[GlobalVariables.Instance.InteractKey].wasPressedThisFrame && InteractButton != null){
            OnButtonClick();
        }
    }
    public void OnButtonClick(){
        Debug.Log("Button Onclick");
    }
    void UpdateText(){
        Text interactMsg = InteractButton.transform.Find("Text").GetComponent<Text>();
        interactMsg.text = "Clerk";
    }
}