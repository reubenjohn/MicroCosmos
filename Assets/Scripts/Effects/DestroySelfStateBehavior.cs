﻿using UnityEngine;

public class DestroySelfStateBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        Destroy(animator.gameObject);
}