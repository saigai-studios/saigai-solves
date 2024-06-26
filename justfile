# Build Saigai Solves for your preferred target system
build UNITY_PATH TARGET:
    cd Rust; cargo run --release; cd ..
    python ./.github/workflows/store-dll.py
    {{UNITY_PATH}} -projectPath "." -batchmode -buildTarget {{TARGET}} -logFile -
