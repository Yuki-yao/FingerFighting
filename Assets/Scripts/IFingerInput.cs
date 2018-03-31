namespace FingerInput {
    public enum ActionType {
        Null, Defend, Jump, Backward, Punch, Forward, Squat, Kick
    }
    interface IFingerInput {
        ActionType GetAction();
    }
}