using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string WALKING_ANIMATION = "IsWalking";
    private Animator animator;
    [SerializeField] private Player player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(WALKING_ANIMATION, player.IsWalking());
    }
}
