---
name: Bug report
about: Found something that doesn't work as documented or completely fails.
title: 'Bug: '
labels: bug
assignees: ''

---

<!--

If you have questions about how to use Json.NET, please read the Json.NET documentation or ask on Stack Overflow.
There are thousands of Json.NET questions on Stack Overflow with the json.net tag.

https://www.newtonsoft.com/json/help
https://stackoverflow.com/questions/tagged/json.net

These GitHub issues are only for reporting bugs with the package itself.

⚠ Please note that this project is released with a Contributor Code of Conduct.
By participating in this project you agree to abide by its terms.
Read more: https://github.com/jilleJr/Newtonsoft.Json-for-Unity/blob/master/CODE_OF_CONDUCT.md

-->

## Expected behavior

<!-- Ex: To read this json on iOS: /followed by code block/ -->

## Actual behavior

<!-- Crash at runtime? Did it work in the Editor? Fails to compile? -->

## Steps to reproduce

- New project
- Import `jillejr.newtonsoft.json-for-unity` via UPM
- Add following script to scene:

```csharp
void Start() {
    // Your calls to Newtonsoft.Json here
}
```

- Run the game

## Details

<!-- Windows/Mac/Linux? What dialect and version? -->
Host machine OS running Unity Editor 👉 SPECIFY

<!-- Windows/Mac/Linux/Android/iOS/WebGL/etc. -->
Unity build target 👉 SPECIFY

<!-- Found in manifest.json & Package Manager window. Ex: 12.0.101 -->
Newtonsoft.Json-for-Unity package version 👉 SPECIFY

<!-- Ex: 2019.1.11f1. Specify multiple if applicable. -->
I was using Unity version 👉 SPECIFY

## Checklist

<!--
Replace the space between the brackets with "x" to mark it as acknowledged. Like so:
- [x] Completed task
-->

- [ ] Shutdown Unity, deleted the /Library folder, opened project again in Unity, and problem still remains.
- [ ] Checked to be using latest version of the package.
