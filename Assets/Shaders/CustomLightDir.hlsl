//main light(장면에 배치된 direction light를 얻는다)의 빝벡터를 얻어오는 함수
void CustomLightDir_half(out half3 t_lightDir)
{
#ifdef SHADERGRAPH_PREVIEW
	t_lightDir = half3(1, 1, 1);
#else
	t_lightDir = GetMainLight().direction;
#endif
}