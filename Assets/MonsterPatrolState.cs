public class MonsterPatrolState : State
{
    private AIMonsterController monster;
    private Parameter param;
    public MonsterPatrolState(AIMonsterController monster)
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