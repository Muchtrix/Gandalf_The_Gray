build:
	dotnet build -c Release

run:
	@dotnet run --no-build -c Release

clean:
	rm -fr bin obj