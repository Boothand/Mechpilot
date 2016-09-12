using UnityEngine;

public class PlayerInput : BaseInput
{
	[SerializeField] private string m_horzString = "Horizontal";
	[SerializeField] private string m_vertString = "Vertical";
	[SerializeField] private string m_crouchString = "Crouch";
	[SerializeField] private string m_fireString = "Fire";
	
	void Update ()
	{
		m_horz = Input.GetAxis(m_horzString);
		m_horzRaw = Input.GetAxisRaw(m_horzString);
		m_vert = Input.GetAxis(m_vertString);
		m_vertRaw = Input.GetAxisRaw(m_vertString);

		m_crouch = Input.GetAxis(m_crouchString);
		m_crouchRaw = Input.GetAxisRaw(m_crouchString);

		m_fire = Input.GetButtonDown(m_fireString);
		m_fireHold = Input.GetButton(m_fireString);
	}
}