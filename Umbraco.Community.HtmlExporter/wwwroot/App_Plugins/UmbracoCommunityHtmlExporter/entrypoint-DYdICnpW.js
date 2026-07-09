import { UMB_AUTH_CONTEXT as i } from "@umbraco-cms/backoffice/auth";
import { c as s } from "./client.gen-miud6JxX.js";
const c = (o, t) => {
  o.consumeContext(i, async (e) => {
    const n = e?.getOpenApiConfiguration();
    s.setConfig({
      auth: n?.token ?? void 0,
      baseUrl: n?.base ?? "",
      credentials: n?.credentials ?? "same-origin"
    });
  });
}, g = (o, t) => {
};
export {
  c as onInit,
  g as onUnload
};
//# sourceMappingURL=entrypoint-DYdICnpW.js.map
