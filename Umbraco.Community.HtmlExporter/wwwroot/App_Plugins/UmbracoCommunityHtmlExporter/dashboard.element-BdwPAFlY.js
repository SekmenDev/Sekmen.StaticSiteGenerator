import { LitElement as D, html as f, css as R, state as S, customElement as V } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as $ } from "@umbraco-cms/backoffice/element-api";
import { UMB_NOTIFICATION_CONTEXT as T } from "@umbraco-cms/backoffice/notification";
import { c as g, f as C } from "./client.gen-miud6JxX.js";
class v {
  static exportWebsite(e) {
    return (e?.client ?? g).post({
      ...C,
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracocommunityhtmlexporter/api/v1/export-website",
      ...e,
      headers: {
        "Content-Type": null,
        ...e?.headers
      }
    });
  }
  static getData(e) {
    return (e?.client ?? g).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/umbracocommunityhtmlexporter/api/v1/get-data",
      ...e
    });
  }
}
var F = Object.defineProperty, O = Object.getOwnPropertyDescriptor, b = (t) => {
  throw TypeError(t);
}, x = (t, e, r, u) => {
  for (var s = u > 1 ? void 0 : u ? O(e, r) : e, o = t.length - 1, n; o >= 0; o--)
    (n = t[o]) && (s = (u ? n(e, r, s) : n(s)) || s);
  return u && s && F(e, r, s), s;
}, E = (t, e, r) => e.has(t) || b("Cannot " + r), l = (t, e, r) => (E(t, e, "read from private field"), e.get(t)), y = (t, e, r) => e.has(t) ? b("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), L = (t, e, r, u) => (E(t, e, "write to private field"), e.set(t, r), r), a, c;
let d = class extends $(D) {
  constructor() {
    super(), y(this, a), y(this, c, async (t) => {
      const e = t.target;
      e.state = "waiting";
      var r = this.shadowRoot?.getElementById("sourceSite")?.value, u = this.shadowRoot?.getElementById("outputFolder")?.value, s = this.shadowRoot?.getElementById("additionalUrls")?.value, o = this.shadowRoot?.getElementById("targetUrl")?.value, n = this.shadowRoot?.getElementById("stringReplacements")?.value;
      if (!r) {
        e.state = "failed", l(this, a) && l(this, a).peek("danger", {
          data: {
            headline: "Error",
            message: "Please select a valid source site URL"
          }
        });
        return;
      }
      if (!u) {
        e.state = "failed", l(this, a) && l(this, a).peek("danger", {
          data: {
            headline: "Error",
            message: "Please enter a valid output folder"
          }
        });
        return;
      }
      if (!o) {
        e.state = "failed", l(this, a) && l(this, a).peek("danger", {
          data: {
            headline: "Error",
            message: "Please enter a valid target URL"
          }
        });
        return;
      }
      o.endsWith("/") || (o += "/"), !o.startsWith("http://") && !o.startsWith("https://") && (o = "http://" + o);
      const w = n.split(`
`).map((i) => i.trim()).filter((i) => i.length > 0 && i.includes("|")).map((i) => {
        const [m, U] = i.split("|", 2);
        return { oldValue: m.trim(), newValue: U?.trim() || "" };
      }), p = {
        SiteUrl: r,
        OutputFolder: u,
        TargetUrl: o,
        AdditionalUrls: s.split(`
`).map((i) => i.trim()).filter((i) => i.length > 0)
      };
      w.forEach((i, m) => {
        p[`StringReplacements[${m}].OldValue`] = i.oldValue, p[`StringReplacements[${m}].NewValue`] = i.newValue;
      });
      const { data: _, error: h } = await v.exportWebsite({
        body: p
      });
      if (h) {
        e.state = "failed", console.error(h), l(this, a) && l(this, a).peek("danger", {
          data: {
            headline: "Export failed",
            message: "An error occurred during export. See console for details."
          }
        });
        return;
      }
      _ !== void 0 && (e.state = "success", l(this, a) && l(this, a).peek("positive", {
        data: {
          headline: "Export finished",
          message: "The HTML export has been finished successfully."
        }
      })), setTimeout(() => {
        e.state = "success";
      }, 2e3);
    }), this.consumeContext(T, (t) => {
      L(this, a, t);
    }), v.getData().then((t) => {
      this._serverDomainData = t.data;
    }).catch((t) => {
      console.error(t), l(this, a) && l(this, a).peek("danger", {
        data: {
          headline: "Error fetching data from server",
          message: "See console for details"
        }
      });
    });
  }
  render() {
    return f`
      <uui-box headline="Export Settings" class="wide">
        <uui-form>
          <uui-form-layout-item>
            <uui-label slot="label" for="sourceSite">Select source site</uui-label>
            <uui-radio-group name="sourceSite" id="sourceSite" required role="radiogroup">
              ${this._serverDomainData?.domains?.map(
      (t, e) => f`<uui-radio name="site" value="${t.url}" checked="${e === 0}">
                    ${t.name} (${t.url})
                  </uui-radio>`
    )}
            </uui-radio-group>
          </uui-form-layout-item>

          <uui-form-layout-item>
            <uui-label slot="label" for="outputFolder">Output Folder</uui-label>
            <uui-input type="text" 
              name="outputFolder"
              id="outputFolder"
              required
              class="full-width"
              placeholder="Enter output folder"
              value="${this._serverDomainData?.settings?.outputFolder || ""}"
            ></uui-input>
          </uui-form-layout-item>

          <uui-form-layout-item>
            <uui-label slot="label" for="additionalUrls">
              Additional URLs (one per line)
            </uui-label>
            <uui-textarea
              name="additionalUrls"
              id="additionalUrls"
              placeholder="Enter additional URLs (one per line)"
              rows="5"
              value="${this._serverDomainData?.settings?.additionalUrls.join(`
`) || ""}"
            ></uui-textarea>
          </uui-form-layout-item>

          <uui-form-layout-item>
            <uui-label slot="label" for="targetUrl">
              Target URL
            </uui-label>
            <uui-input type="text" 
              name="targetUrl" 
              id="targetUrl" 
              required 
              class="full-width"
              placeholder="Enter target URL"
              value="${this._serverDomainData?.settings?.targetUrl || ""}"
            ></uui-input>
          </uui-form-layout-item>

          <uui-form-layout-item>
            <uui-label slot="label" for="stringReplacements">
              String Replacements (Format: oldValue|newValue, one per line)
            </uui-label>
            <uui-textarea
              name="stringReplacements"
              id="stringReplacements"
              placeholder="Enter string replacements (format: oldValue|newValue, one per line)"
              rows="5"
              value="${this._serverDomainData?.settings?.stringReplacements?.map((t) => `${t.oldValue}|${t.newValue}`).join(`
`) || ""}"
            ></uui-textarea>
          </uui-form-layout-item>
          <uui-button
            color="default"
            look="primary"
            @click="${l(this, c)}"
          >
            Export HTML
          </uui-button>
        </uui-form>
      </uui-box>
    `;
  }
};
a = /* @__PURE__ */ new WeakMap();
c = /* @__PURE__ */ new WeakMap();
d.styles = [
  R`
      :host {
        display: grid;
        gap: var(--uui-size-layout-1);
        padding: var(--uui-size-layout-1);
        grid-template-columns: 1fr 1fr 1fr;
      }

      uui-box {
        margin-bottom: var(--uui-size-layout-1);
      }

      uui-input.full-width {
        width: 100%;
      }

      h2 {
        margin-top: 0;
      }

      .wide {
        grid-column: span 3;
      }
    `
];
x([
  S()
], d.prototype, "_serverDomainData", 2);
d = x([
  V("html-exporter-dashboard")
], d);
const P = d;
export {
  d as HtmlExporterDashboardElement,
  P as default
};
//# sourceMappingURL=dashboard.element-BdwPAFlY.js.map
