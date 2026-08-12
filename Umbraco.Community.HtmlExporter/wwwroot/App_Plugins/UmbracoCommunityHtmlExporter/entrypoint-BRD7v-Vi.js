import { UMB_AUTH_CONTEXT as o } from "@umbraco-cms/backoffice/auth";
import { c as i } from "./client.gen-miud6JxX.js";
const r = async (t, e) => {
  const n = await t.getContext(o);
  if (!n) {
    console.warn("UMB_AUTH_CONTEXT not available — extension API client will not be authenticated");
    return;
  }
  n.configureClient(i);
}, c = (t, e) => {
};
export {
  r as onInit,
  c as onUnload
};
//# sourceMappingURL=entrypoint-BRD7v-Vi.js.map
