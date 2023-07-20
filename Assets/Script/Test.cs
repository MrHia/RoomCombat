using UnityEngine;

public class Test : MonoBehaviour {
    private Rigidbody[] _rigidbodysRagdoll;
    private Collider[] _collidersRagdoll;

    private Animator _animator;

    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] Collider _collider;

    private void Start() {
        _animator = GetComponentInChildren<Animator>();
        _rigidbodysRagdoll = GetComponentsInChildren<Rigidbody>();
        _collidersRagdoll = GetComponentsInChildren<Collider>();
        foreach (Rigidbody rigidbody in _rigidbodysRagdoll) {
            rigidbody.isKinematic = true;
        }
        //_rigidbody = GetComponent<Rigidbody>();
    }
    private void Update() {

    }
    private void OnEnable() {

        Invoke("OnRagDoll", 0.5f);
    }
    private void OnRagDoll() {
        _animator.enabled = false;
        _rigidbody.detectCollisions = false;
        _collider.enabled = false;
        foreach (Rigidbody rigidbody in _rigidbodysRagdoll) {
            rigidbody.isKinematic = false;
        }
        Invoke("Destroy", 2f);
    }
    public void Destroy() {
        foreach (Collider collider in _collidersRagdoll) {
            collider.isTrigger = true;
        }
        Destroy(gameObject,3f);
    }
}
