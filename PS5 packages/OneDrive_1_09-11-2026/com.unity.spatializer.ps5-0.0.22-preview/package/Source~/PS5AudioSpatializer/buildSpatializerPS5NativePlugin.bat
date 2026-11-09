"%SCE_PROSPERO_SDK_DIR%\host_tools\bin\prospero-clang" -oPS5AudioSpatializer.prx *.cpp -D LIBRARY_IMPL -O3 -g --for-linker=--oformat=prx -lSceAudioOut2_stub_weak -lScePosix_stub_weak

