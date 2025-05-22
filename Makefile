.PHONY: all lint clean test build

lint:
	@echo "Formatting code..."
	cd src && dotnet format --verify-no-changes

build: lint
	@echo "Building..."
	cd src && dotnet build --configuration Release

run:
	@echo "Running..."
	cd src && dotnet run
