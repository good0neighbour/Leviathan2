//main light(��鿡 ��ġ�� direction light�� ��´�)�� �����͸� ������ �Լ�
void CustomLightDir_half(out half3 t_lightDir)
{
#ifdef SHADERGRAPH_PREVIEW
	t_lightDir = half3(1, 1, 1);
#else
	t_lightDir = GetMainLight().direction;
#endif
}