namespace Domain.Citizens.States
{
    public enum CitizenState
    {
        Idle,
        GoingHome,
        GoingWork,
        Working,
        GoingToStudy,
        Studying,
        WorkingOnSite, // работник на выездном задании
        GoingToCommercial,
        AtCommercial,
        GoingHomeFromCommercial,
    }

}
