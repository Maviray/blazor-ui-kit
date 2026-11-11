# Changelog

Changes to the Maviray.Blazor.Components packages (Core, Material, Tailwind).

## 0.3.0 (unreleased)

### Upgrade notes

Check these when you update an app:

- **Text, number and date inputs update their bound value on change, not on every keystroke.** The value is applied when
  the user leaves the field or presses Enter, like Blazor's built-in inputs. Set `Immediate="true"` on an input that
  must update while typing. `MaviTextArea` keeps live updates while `ShowCharacterCount` is on.
- **Validation runs once per change** instead of on every keystroke.
- **Lifecycle traces log at Debug** (they logged at Information). With `EnableLifecycleLogging` on, set the `Maviray`
  log category to Debug to see them.
- **`WysiwygEditor` sanitizes HTML** going into and out of the editor with HtmlSanitizer. Links it inserts must be
  http(s), mailto, tel or relative URLs. To change the sanitizer policy, register your own `Ganss.Xss.IHtmlSanitizer`
  in DI.
- **Blazor WebAssembly Release builds now trim these libraries** (they are marked trimmable). Run a Release publish of
  your WebAssembly app once and click through it.
- **`IconSolidType.DocumentCheckSolid` renders an icon** (it rendered nothing).

### Look and feel

- Dropdown and MultiSelect option lists have a thin gray scrollbar. The components already used these scrollbar classes,
  but Tailwind didn't generate them until 4.3.
- Icon buttons center their icon.
- A focused read-only field shows the accent label, icon and 2px outline again.
- Floating labels and focus styling come from CSS, so text inputs and AutoComplete no longer make a server round-trip
  on focus and blur.
- The default sans-serif font stack is unchanged: Tailwind 4.3 changed its default, and the stylesheets keep the 4.1
  one.

### Fixed

- Tabs: `OnTabClick` fired once per earlier re-render of the parent; it now fires once per click.
- Inputs: removed inputs detach from the `EditContext` (each input leaked a handler; date inputs leaked two).
- Numeric and date inputs format with `CurrentCulture` instead of `CurrentUICulture`; negative numbers no longer render
  blank in cultures such as sv-SE.
- AutoComplete: stale search results are dropped, and an endless render loop in hidden containers is gone.
- Toastr: auto-dismiss timers are cancelled when the component is disposed.
- Tables: enum columns show `[Display]` names; the global search refilters; filtering returns to page 1; the table
  re-renders when the parent passes new parameters; `long` values beyond the `int` range sort correctly; missing cells
  and null values sort after all values when ascending (first when descending, which reverses the order).
- Popups listen for outside clicks only while open.
- Date inputs: trimming reports no warnings for their JavaScript callbacks.

### Performance

- Inputs send far fewer events to the server (change-based binding, CSS focus styling).
- `CssManager.Combine` gains a `params ReadOnlySpan<string?>` overload that allocates only its result; the array
  overload uses it.
- Icons are looked up with a switch instead of reflection, and their path lists are built once.
- Tables look cells up by column key (`MaviTableRow.GetCell`), compute sort keys once per row, and render row menus only
  while open.

### Dependencies

- Microsoft.AspNetCore.Components, Components.Forms, Components.Web, Microsoft.JSInterop and
  Microsoft.Extensions.Logging.Abstractions 10.0.15.
- HtmlSanitizer 11.0.26.
- The stylesheets are built with Tailwind CSS 4.3.5 and @tailwindcss/typography 0.5.20. They ship prebuilt in the
  packages, so apps install nothing.

### Building this repository

- Shared library settings live in `src/Directory.Build.props`, and package versions in `Directory.Packages.props`.
- The .NET analyzers run at level 10.0, and warnings fail the build.
- The Tailwind CSS builds only when its inputs change, including in Visual Studio. `-p:SkipTailwindBuild=true` builds
  without Node.js and uses the committed CSS.
