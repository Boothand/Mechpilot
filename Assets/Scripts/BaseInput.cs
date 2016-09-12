using UnityEngine;


//!This class is used to provide input to any RigidbodyController class - moving thinking objects.
/*!It exists so that players and AI will use the same code and same rules for their actions.
*/

public class BaseInput : MonoBehaviour
{
	protected float m_horz;
	protected float m_horzRaw;
	protected float m_vert;
	protected float m_vertRaw;

	protected float m_mouseX;
	protected float m_mouseY;

	protected float m_crouch;
	protected float m_crouchRaw;

	protected bool m_fire;
	protected bool m_fireHold;

	public float Horz { get { return m_horz; } }
	public float HorzRaw { get { return m_horzRaw; } }
	public float Vert { get { return m_vert; } }
	public float VertRaw { get { return m_vertRaw; } }

	public float Crouch { get { return m_crouch; } }
	public float CrouchRaw { get { return m_crouchRaw; } }

	public bool Fire { get { return m_fire; } }
	public bool FireHold { get { return m_fireHold; } }
}