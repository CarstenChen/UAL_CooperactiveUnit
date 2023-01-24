public class MonsterChaseState : State
{
    private AIMonsterController monster;
    private Parameter param;
    public MonsterChaseState(AIMonsterController monster)
    {
        this.monster = monster;
        this.param = monster.param;
    }

    public void OnStateEnter()
    {

    }
    public void OnStateStay()
    {

    }

    public void OnStateExit()
    {

    }
}