{
  description = "TempleLang compiler";

  inputs.nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";

  outputs = {
    self,
    nixpkgs,
  }: let
    supportedSystems = [
      "x86_64-linux"
      "aarch64-linux"
    ];
    forAllSystems = nixpkgs.lib.genAttrs supportedSystems;
  in {
    packages = forAllSystems (
      system: let
        pkgs = nixpkgs.legacyPackages.${system};
      in {
        default = pkgs.buildDotnetModule {
          pname = "TempleLang";
          version = "0.1.0";

          src = ./src;

          projectFile = "TempleLang.CLI/TempleLang.CLI.csproj";
          nugetDeps = ./nix/deps.json;

          # The code is targeting .NET Core 3.1,
          # so any stone-age sdk will do.
          # We just use the latest available version
          # because why not
          dotnet-sdk = pkgs.dotnetCorePackages.sdk_11_0;
          dotnet-runtime = pkgs.dotnetCorePackages.runtime_11_0;

          executables = ["TempleLang.CLI"];

          # Bundle nasm and gcc so the compiler can call them at runtime
          makeWrapperArgs = [
            "--prefix"
            "PATH"
            ":"
            (pkgs.lib.makeBinPath [
              pkgs.nasm
              # Used as a linker driver, no actual C compiler features are used.
              # This just saves us from looking up libc and the dynamic linker path ourselves.
              pkgs.gcc
            ])
          ];

          meta = {
            description = "A compiled, statically-typed toy language targeting x86-64";
            license = pkgs.lib.licenses.mit;
            mainProgram = "TempleLang.CLI";
            platforms = supportedSystems;
          };
        };
      }
    );

    devShells = forAllSystems (
      system: let
        pkgs = nixpkgs.legacyPackages.${system};
      in {
        default = pkgs.mkShell {
          packages = [
            pkgs.dotnetCorePackages.sdk_11_0
            pkgs.nasm
            pkgs.gcc
          ];
        };
      }
    );
  };
}
