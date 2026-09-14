# Reborn

Unity HDRP 기반 1인칭 내러티브 게임.

## 개발 기준

- Unity: 6000.6.0f1
- HDRP: 17.7.0
- 배포 대상: Windows, macOS
- 패키지 버전 기준: Packages/manifest.json 및 Packages/packages-lock.json

## 폴더 구성

| 경로 | 용도 |
| --- | --- |
| Assets/Reborn/Scenes | 직접 제작하는 게임 씬 |
| Assets/Reborn/Scripts | 게임 C# 코드. 에디터 전용 코드는 필요 시 하위 Editor 폴더에 배치 |
| Assets/Reborn/Prefabs | 상호작용 오브젝트와 재사용 프리팹 |
| Assets/Reborn/Materials | 게임 공통 재질 |
| Assets/Reborn/Art/Environment | 공간 모델과 해당 텍스처 |
| Assets/Reborn/Art/Props | 소품 모델과 해당 텍스처 |
| Assets/Reborn/Art/Documents | 일기·서류·사진 등 이야기용 이미지 |
| Assets/Reborn/Animation | 애니메이션 클립·컨트롤러·Timeline |
| Assets/Reborn/Audio/SFX | 동작 및 상호작용 효과음 |
| Assets/Reborn/Audio/Ambience | 공간 환경음 |
| Assets/Reborn/Audio/Voice | 대사 및 인물 음성 |
| Assets/Reborn/Audio/Music | 음악 |
| Assets/Reborn/UI | UI 레이아웃·폰트·아이콘 |
| Assets/Reborn/Data | 게임 설정과 진행 데이터 정의. 플레이어 저장 파일은 제외 |
| Assets/ThirdParty | 외부 에셋. 제공자가 고정 경로를 요구하면 원래 경로 유지 |
| SourceAssets | Blender 등 편집 원본. Unity 자동 임포트 대상 밖에 보관 |
| Documentation/asset-register.md | 에셋 출처와 사용 조건 기록 |

폴더의 .gitkeep은 빈 폴더 유지용. 실제 파일 추가 후 제거 가능.
Unity의 .meta는 원본 파일과 함께 버전 관리하며 이동 시에도 함께 이동.
제작 원본은 SourceAssets, 게임에서 사용하는 출력물은 Assets에 배치.
외부 모델의 HDRP 재질과 상호작용 변형은 Assets/Reborn에서 관리.
기본 템플릿 씬과 Settings는 현재 위치 유지.

## 기획 자료

이 작업공간에서는 저장소 밖 ../../docs 및 ../../art/concepts에 기획 문서와 컨셉 이미지 보관.
해당 자료는 현재 GitHub 저장소에 포함되지 않으므로 이 저장소만 복제하면 함께 내려받아지지 않음.
제작 목록 기준: ../../docs/development/08-production-inventory.md.

## 변경 관리

- 커밋 전 작업 결과 보고 및 사용자 컨펌 필요
- 커밋 제목은 영어, 본문은 한글 명사형 사용: “폴더 구조 추가”, “배치 기준 구성”
- 작업 단위별 관련 변경만 커밋
- .gitattributes 기준 모델·이미지·오디오의 Git LFS 적용 확인
- Library, Temp, Logs 등 생성 파일은 .gitignore로 제외

현재 단계는 폴더 구조 구성. 플레이 가능한 씬, 조작, 상호작용 및 CLI 자동화는 후속 작업.
