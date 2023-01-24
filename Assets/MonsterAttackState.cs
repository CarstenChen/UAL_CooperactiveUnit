public class MonsterAttackState : State
{
    private AIMonsterController monster;
    private Parameter param;
    public MonsterAttackState(AIMonsterController monster)
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