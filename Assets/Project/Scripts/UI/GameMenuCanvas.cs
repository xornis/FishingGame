using UnityEngine;

public class GameMenuCanvas : UICanvas
{
    [SerializeField] private PlayerStatsUI statsPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show()
    {
        base.Show();
        statsPanel.gameObject.SetActive(true);
    }
}
