# RoadKill

> Unity 기반 협업 프로젝트 **RoadKill** 저장소입니다.  
> 원활한 협업과 에셋 충돌 방지를 위해 아래 가이드를 반드시 준수해 주세요.
> 잘 모르겠다 싶으면 GPT나 Gemini같은데 물어보세요.

---

## 1. 최초 프로젝트 세팅 (팀원 필수)

### 1) Git LFS 확인 및 활성화
에셋(이미지, 오디오, 3D 모델 등) 바이너리 파일 관리를 위해 **Git LFS**가 필수입니다.
```bash
# Git LFS 설치 상태 확인 및 활성화 (최초 1회)
git lfs install

```

### 2) 프로젝트 클론

```bash
git clone [https://github.com/AXF4/RoadKill.git](https://github.com/AXF4/RoadKill.git)
cd RoadKill

```

### 3) Unity Hub에서 프로젝트 열기

1. **Unity Hub** 실행 ➔ 우측 상단 **Add (디스크에서 프로젝트 추가)** 클릭
2. 클론받은 `RoadKill` 폴더 선택
3. 지정된 Unity 에디터 버전으로 실행
*(최초 로딩 시 `Library/` 캐시 폴더를 생성하므로 수 분 정도 소요될 수 있습니다.)*

---

## 2. 작업 워크플로우

`main` 브랜치는 항상 빌드 및 실행이 가능한 상태를 유지합니다. **모든 작업은 브랜치를 분기하여 진행합니다.**

```
main ──────────────●───────────● (Merge)
       \                      /
feat/player ────●──────●─────┘ (PR)

```

### 작업 단계

1. **최신 코드 가져오기:**
```bash
git checkout main
git pull origin main

```


2. **새 기능 브랜치 생성 및 이동:**
```bash
git checkout -b <브랜치타입>/<기능이름>
# 예시: git checkout -b feat/player-movement

```


* **브랜치 네이밍 컨벤션:**
* `feat/기능명` : 신규 기능 개발
* `fix/버그명` : 버그 수정
* `refactor/작업명` : 코드 리팩토링
* `docs/문서명` : 문서 작업




3. **작업 후 커밋:**
* 가급적 **Unity 에디터 저장 후 종료된 상태**에서 커밋 권장


```bash
git status  # 변경된 파일 목록 및 .meta 포함 여부 확인
git add .
git commit -m "feat: 플레이어 이동 로직 구현"

```


* **커밋 태그:** `feat`, `fix`, `refactor`, `style`, `docs`, `chore`


4. **원격 저장소에 푸시:**
```bash
git push origin <브랜치이름>

```



---

## 3. Pull Request (PR) 및 머지

1. GitHub [AXF4/RoadKill](https://www.google.com/search?q=https://github.com/AXF4/RoadKill) 접속
2. 상단 **Compare & pull request** 버튼 클릭 (또는 **Pull requests** 탭 ➔ **New pull request**)
3. `base: main` ➔ `compare: feat/본인브랜치` 설정
4. **PR 템플릿 작성:**
* 구현 내용 간략 요약
* 테스트 완료 여부 (스크린샷/GIF 첨부 권장)


5. **Create pull request** 클릭
6. 팀원 리뷰/확인 후 `main`으로 Merge 진행
7. 머지 완료된 로컬 및 원격 브랜치는 정리(삭제)

---

## 4. 절대 하지 말아야 할 행동 (주의사항)

### ❌ `main` 브랜치에 직접 커밋/푸시 금지

* `main`에는 직접 push하지 마세요. 모든 작업은 브랜치를 따서 PR을 통해 머지합니다.

### ❌ 공용 씬(`SampleScene.unity` 등) 동시 수정 금지

* 씬 파일은 YAML 텍스트 구조상 여러 작업자가 동시에 건드리면 **자동 머지가 불가능**에 가깝습니다.
* **해결책:**
* 모든 기능(플레이어, 몬스터, UI 등)은 **프리팹(Prefab)** 단위로 모듈화하여 작업하세요.
* 개인 작업용 테스트 씬(`Assets/Scenes/Test/Test_홍길동.unity`)을 별도로 만들어 작업하고, 메인 씬에는 완성된 프리팹만 가져다 붙입니다.



### ❌ `.meta` 파일 누락 및 단독 조작 금지

* Unity 에셋은 실제 파일과 1:1로 매칭되는 `.meta` 파일(고유 GUID)이 생명입니다.
* 파일 추가, 이름 변경, 이동, 삭제는 **반드시 Unity 에디터의 Project 뷰 안에서** 수행하세요.
* `git add` 시 `.meta` 파일이 누락되면 다른 팀원의 프로젝트에서 참조가 전부 깨집니다(`Missing Reference`).

### ❌ 임시/캐시 파일 강제 커밋 금지

* `Library/`, `Temp/`, `Logs/`, `obj/`, `*.csproj` 등은 로컬 빌드용 임시 파일입니다.
* `.gitignore` 설정을 우회하여 강제로 추가(`git add -f`)하지 마세요.

### ❌ Unity 에디터 버전 임의 변경 금지

* 프로젝트 설정 파일이나 패키지 직렬화가 꼬일 수 있으므로 정해진 버전 외 다른 버전으로 프로젝트를 업그레이드하여 열지 마세요.
