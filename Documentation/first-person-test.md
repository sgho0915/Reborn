# 2단계 — 1인칭 기본 조작

## 실행

1. Assets/Reborn/Scenes/FirstPersonTest.unity 열기
2. Unity Play 버튼 선택
3. Game 화면 클릭 후 WASD 이동 및 마우스 회전
4. Esc로 조작 해제, Game 화면 클릭으로 복귀

시작 시 커서를 강제로 잡지 않음. 다른 앱으로 전환하면 조작 해제.
실행 종료 시 커서 복원. 문서 조사나 메뉴 연동은 후속 단계.

## 설정

Player의 FirstPersonController에서 Move Speed와 Look Sensitivity 조절.
기본 속도 1.6m/s, 감도 0.1도/마우스 델타 단위, 상하 회전 제한 ±80도.
PlayerCamera 높이 1.6m, 수직 FOV 60도.
점프·달리기·카메라 흔들림 없음. 감도 저장과 키 재지정 UI는 후속 단계.
Assets/Reborn/Prefabs/FirstPersonPlayer.prefab을 이후 원룸 씬에서 재사용 가능.

## 검증 결과

Unity 6000.6.0f1, HDRP 17.7.0, macOS 에디터에서 확인.

- 컴파일 오류 없음
- Play 모드 진입 및 HDRP 카메라 출력 확인
- 1초 전진 거리 1.6m 허용 오차 내 통과
- 대각선 속도 정규화 및 30/60fps 이동량 비교 통과
- 바닥 접촉, 전방 장애물 충돌, 폭 1m 문 통과 검증 통과
- 상하 회전 제한, 좌우 회전, 카메라 롤 없음, FOV·높이 검증 통과

자동 검증은 이동·시점 함수를 직접 호출하는 방식.
실제 장치 입력부터의 전체 경로, Esc/클릭 및 앱 전환, 체감 감도는 수동 검증 필요.
Windows 실행 파일 검증은 아직 미수행.

## 재검증

테스트 씬 Play 모드에서 프로젝트 디렉터리를 기준으로 실행:

```sh
unity command eval 'return Reborn.Editor.FirstPersonValidation.Run();' --json
```

기본 설정용 검증이며 테스트 중 플레이어 위치 변경 후 복구.
테스트 공간은 조작 검증용으로 실제 원룸의 배치·크기·시각 품질과 무관.
기존 빌드 씬 목록은 유지. 테스트 씬은 에디터에서 직접 열어 실행.
