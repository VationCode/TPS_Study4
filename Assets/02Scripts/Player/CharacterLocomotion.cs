using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private Animator _anim;
    private Vector2 _input;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");

        _anim.SetFloat("InputX", _input.x);
        _anim.SetFloat("InputY", _input.y);
    }
}
