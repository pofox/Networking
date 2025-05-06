using UnityEngine;

public class Projectail : MonoBehaviour
{
    [SerializeField] Vector3 Speed;
    [SerializeField] Transform Target;
    [SerializeField] Vector3 Targetdir;
    [SerializeField] Vector3 TargetSpeed;
    [SerializeField] Vector3 Targetprevpos;
    [SerializeField] float gravity = 9.8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TargetSpeed = (Target.position - Targetprevpos) / Time.deltaTime;
        Targetdir = TargetSpeed.normalized;

        //calculate speed

        //F=ma -> -mg=ma -> a=-g -> v=-gt+c1 -> v=-gt+v0 -> x=-gt^2+v0t+c2 ->

        //move projectail to target
        transform.position += Speed * Time.deltaTime + Vector3.down * gravity;

        //save prev position
        Targetprevpos = Target.position;
    }
}
