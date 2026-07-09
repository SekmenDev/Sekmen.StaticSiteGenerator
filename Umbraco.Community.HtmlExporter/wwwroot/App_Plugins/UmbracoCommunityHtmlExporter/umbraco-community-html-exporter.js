const t = [
  {
    name: "Umbraco Community Html Exporter Entrypoint",
    alias: "Umbraco.Community.HtmlExporter.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-DYdICnpW.js")
  }
], o = [
  {
    name: "Umbraco Community Html Exporter Dashboard",
    alias: "Umbraco.Community.HtmlExporter.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-BdwPAFlY.js"),
    meta: {
      label: "HTML Exporter",
      pathname: "html-exporter-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  }
], a = [
  ...t,
  ...o
];
export {
  a as manifests
};
//# sourceMappingURL=umbraco-community-html-exporter.js.map
