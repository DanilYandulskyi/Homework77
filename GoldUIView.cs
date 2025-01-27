using UnityEngine;

[RequireComponent(typeof(GoldUI))]
public class GoldUIView : MonoBehaviour
{
    [SerializeField] private Base _base;

    private GoldUI _ui;

    private void Start()
    {
        _ui = GetComponent<GoldUI>();
        _ui.UpdateText(0);

        _base.GoldAmountChanged += IncreaseGoldAmount;
    }

    private void IncreaseGoldAmount()
    {
        _ui.UpdateText(_base.CollectedResounrseAmount);
    }

    public GoldUIView Initialize(Base @base)
    {
        _base = @base;
        return this;
    }

    private void OnDestroy()
    {
        _base.GoldAmountChanged -= IncreaseGoldAmount;
    }
}
