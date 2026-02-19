using UnityEngine;

public class GameMenuCanvas : UICanvas
{
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private StatsManager statsManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show()
    {
        base.Show();
        statsPanel.SetActive(true);

        statsManager.RefreshAllStats();
    }
}
