using TMPro;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
    public TextMeshProUGUI AmmoTMP;

    public void Refresh(int p_ammoCount)
    {
        AmmoTMP.text = p_ammoCount.ToString();
    }
}
