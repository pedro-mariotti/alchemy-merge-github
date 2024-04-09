using UnityEngine;

public class DealButtonTap : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public int increment = 2;//multiplicador do preço do dealCards
    private PCSettings PC;
    private SoundController soundController;


    [Header("Tutorial:")]
    public GameObject objOnboarding1;
    public GameObject objMaoOnboarding1;

    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PCSettings>();
        boxCollider = GetComponent<BoxCollider2D>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

    }
    private void Update()
    {
        //Some tutorial:
        if (PCSettings.onboardingStage>=2)
        {
            objOnboarding1.SetActive(false);
            objMaoOnboarding1.SetActive(false);
        }
        
        // Checa se o player tocou no botão
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !PCSettings.lockGame)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);


            if (boxCollider == Physics2D.OverlapPoint(touchPos))
            {
                Debug.Log("Clica deal");
                int oldPrice = transform.parent.GetComponent<DealController>().dealPrice;
                int newPrice = transform.parent.GetComponent<DealController>().dealPrice + increment;
                //Salva:
                PlayerPrefs.SetInt("dealPrice", newPrice);

                if (transform.parent.GetComponent<DealController>().UpdateEmptyCards())//verifico se o usuario tem grana ou se tem slot vazio para ser preenchido
                {
                    if (FindObjectOfType<BankController>().GetBankValue() >= oldPrice)
                    {
                        transform.parent.GetComponent<DealController>().startDealFigures();
                        FindObjectOfType<BankController>().RemoveMoney(transform.parent.GetComponent<DealController>().dealPrice);
                        transform.parent.GetComponent<DealController>().dealPrice = newPrice;
                        //Salva:
                        PlayerPrefs.SetInt("dealPrice", newPrice);

                        transform.parent.GetComponent<DealController>().updateTextValor(false);
                        soundController.TriggerButtonSound();
                        soundController.TriggerDealSound();
                        Debug.Log("Deal Figures");
                    }
                    else
                    {
                        transform.parent.GetComponent<DealController>().startDealFigures();
                        //transform.parent.GetComponent<DealController>().dealPrice = oldPrice;
                        //Salva:
                        //PlayerPrefs.SetInt("dealPrice", newPrice);

                        transform.parent.GetComponent<DealController>().updateTextValor(false);

                        Debug.Log("Deal Figures de graça");
                    }
                }
                else
                {
                    Debug.Log("Sem slots para preencher ou sem grana");
                }



            }
        }
        if (transform.parent.GetComponent<DealController>().dealPrice > FindObjectOfType<BankController>().GetBankValue() && transform.parent.GetComponent<DealController>().UpdateEmptyCards())
        {
            transform.parent.GetComponent<DealController>().updateTextValor(true);
        }
    }
}
