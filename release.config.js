module.exports = {
	branches: ["main", {"name": "dev", "prerelease": true}],
	repositoryUrl: "https://github.com/Vegari-1/MessageService",
	plugins: [
		"@semantic-release/commit-analyzer",
		"@semantic-release/release-notes-generator",
		"@semantic-release/github",
		"semantic-release-export-data"
	]
}