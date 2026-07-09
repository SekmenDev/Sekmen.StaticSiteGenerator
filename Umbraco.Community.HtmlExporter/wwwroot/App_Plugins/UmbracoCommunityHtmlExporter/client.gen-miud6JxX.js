const N = (r, e, t) => {
  typeof t == "string" || t instanceof Blob ? r.append(e, t) : t instanceof Date ? r.append(e, t.toISOString()) : r.append(e, JSON.stringify(t));
}, de = {
  bodySerializer: (r) => {
    const e = new FormData();
    return Object.entries(r).forEach(([t, a]) => {
      a != null && (Array.isArray(a) ? a.forEach((n) => N(e, t, n)) : N(e, t, a));
    }), e;
  }
}, G = {
  bodySerializer: (r) => JSON.stringify(
    r,
    (e, t) => typeof t == "bigint" ? t.toString() : t
  )
}, M = ({
  onRequest: r,
  onSseError: e,
  onSseEvent: t,
  responseTransformer: a,
  responseValidator: n,
  sseDefaultRetryDelay: f,
  sseMaxRetryAttempts: c,
  sseMaxRetryDelay: i,
  sseSleepFn: o,
  url: d,
  ...s
}) => {
  let u;
  const E = o ?? ((l) => new Promise((y) => setTimeout(y, l)));
  return { stream: async function* () {
    let l = f ?? 3e3, y = 0;
    const S = s.signal ?? new AbortController().signal;
    for (; !S.aborted; ) {
      y++;
      const x = s.headers instanceof Headers ? s.headers : new Headers(s.headers);
      u !== void 0 && x.set("Last-Event-ID", u);
      try {
        const j = {
          redirect: "follow",
          ...s,
          body: s.serializedBody,
          headers: x,
          signal: S
        };
        let m = new Request(d, j);
        r && (m = await r(d, j));
        const p = await (s.fetch ?? globalThis.fetch)(m);
        if (!p.ok)
          throw new Error(
            `SSE failed: ${p.status} ${p.statusText}`
          );
        if (!p.body) throw new Error("No body in SSE response");
        const g = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let C = "";
        const $ = () => {
          try {
            g.cancel();
          } catch {
          }
        };
        S.addEventListener("abort", $);
        try {
          for (; ; ) {
            const { done: L, value: F } = await g.read();
            if (L) break;
            C += F;
            const I = C.split(`

`);
            C = I.pop() ?? "";
            for (const J of I) {
              const _ = J.split(`
`), A = [];
              let k;
              for (const b of _)
                if (b.startsWith("data:"))
                  A.push(b.replace(/^data:\s*/, ""));
                else if (b.startsWith("event:"))
                  k = b.replace(/^event:\s*/, "");
                else if (b.startsWith("id:"))
                  u = b.replace(/^id:\s*/, "");
                else if (b.startsWith("retry:")) {
                  const D = Number.parseInt(
                    b.replace(/^retry:\s*/, ""),
                    10
                  );
                  Number.isNaN(D) || (l = D);
                }
              let z, B = !1;
              if (A.length) {
                const b = A.join(`
`);
                try {
                  z = JSON.parse(b), B = !0;
                } catch {
                  z = b;
                }
              }
              B && (n && await n(z), a && (z = await a(z))), t?.({
                data: z,
                event: k,
                id: u,
                retry: l
              }), A.length && (yield z);
            }
          }
        } finally {
          S.removeEventListener("abort", $), g.releaseLock();
        }
        break;
      } catch (j) {
        if (e?.(j), c !== void 0 && y >= c)
          break;
        const m = Math.min(
          l * 2 ** (y - 1),
          i ?? 3e4
        );
        await E(m);
      }
    }
  }() };
}, Q = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, K = (r) => {
  switch (r) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, X = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, W = ({
  allowReserved: r,
  explode: e,
  name: t,
  style: a,
  value: n
}) => {
  if (!e) {
    const i = (r ? n : n.map((o) => encodeURIComponent(o))).join(K(a));
    switch (a) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${t}=${i}`;
      case "simple":
        return i;
      default:
        return `${t}=${i}`;
    }
  }
  const f = Q(a), c = n.map((i) => a === "label" || a === "simple" ? r ? i : encodeURIComponent(i) : q({
    allowReserved: r,
    name: t,
    value: i
  })).join(f);
  return a === "label" || a === "matrix" ? f + c : c;
}, q = ({
  allowReserved: r,
  name: e,
  value: t
}) => {
  if (t == null)
    return "";
  if (typeof t == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${e}=${r ? t : encodeURIComponent(t)}`;
}, H = ({
  allowReserved: r,
  explode: e,
  name: t,
  style: a,
  value: n,
  valueOnly: f
}) => {
  if (n instanceof Date)
    return f ? n.toISOString() : `${t}=${n.toISOString()}`;
  if (a !== "deepObject" && !e) {
    let o = [];
    Object.entries(n).forEach(([s, u]) => {
      o = [
        ...o,
        s,
        r ? u : encodeURIComponent(u)
      ];
    });
    const d = o.join(",");
    switch (a) {
      case "form":
        return `${t}=${d}`;
      case "label":
        return `.${d}`;
      case "matrix":
        return `;${t}=${d}`;
      default:
        return d;
    }
  }
  const c = X(a), i = Object.entries(n).map(
    ([o, d]) => q({
      allowReserved: r,
      name: a === "deepObject" ? `${t}[${o}]` : o,
      value: d
    })
  ).join(c);
  return a === "label" || a === "matrix" ? c + i : i;
}, Y = /\{[^{}]+\}/g, Z = ({ path: r, url: e }) => {
  let t = e;
  const a = e.match(Y);
  if (a)
    for (const n of a) {
      let f = !1, c = n.substring(1, n.length - 1), i = "simple";
      c.endsWith("*") && (f = !0, c = c.substring(0, c.length - 1)), c.startsWith(".") ? (c = c.substring(1), i = "label") : c.startsWith(";") && (c = c.substring(1), i = "matrix");
      const o = r[c];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        t = t.replace(
          n,
          W({ explode: f, name: c, style: i, value: o })
        );
        continue;
      }
      if (typeof o == "object") {
        t = t.replace(
          n,
          H({
            explode: f,
            name: c,
            style: i,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        t = t.replace(
          n,
          `;${q({
            name: c,
            value: o
          })}`
        );
        continue;
      }
      const d = encodeURIComponent(
        i === "label" ? `.${o}` : o
      );
      t = t.replace(n, d);
    }
  return t;
}, ee = ({
  baseUrl: r,
  path: e,
  query: t,
  querySerializer: a,
  url: n
}) => {
  const f = n.startsWith("/") ? n : `/${n}`;
  let c = (r ?? "") + f;
  e && (c = Z({ path: e, url: c }));
  let i = t ? a(t) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (c += `?${i}`), c;
};
function te(r) {
  const e = r.body !== void 0;
  if (e && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (e)
    return r.body;
}
const re = async (r, e) => {
  const t = typeof e == "function" ? await e(r) : e;
  if (t)
    return r.scheme === "bearer" ? `Bearer ${t}` : r.scheme === "basic" ? `Basic ${btoa(t)}` : t;
}, R = ({
  allowReserved: r,
  array: e,
  object: t
} = {}) => (n) => {
  const f = [];
  if (n && typeof n == "object")
    for (const c in n) {
      const i = n[c];
      if (i != null)
        if (Array.isArray(i)) {
          const o = W({
            allowReserved: r,
            explode: !0,
            name: c,
            style: "form",
            value: i,
            ...e
          });
          o && f.push(o);
        } else if (typeof i == "object") {
          const o = H({
            allowReserved: r,
            explode: !0,
            name: c,
            style: "deepObject",
            value: i,
            ...t
          });
          o && f.push(o);
        } else {
          const o = q({
            allowReserved: r,
            name: c,
            value: i
          });
          o && f.push(o);
        }
    }
  return f.join("&");
}, se = (r) => {
  if (!r)
    return "stream";
  const e = r.split(";")[0]?.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json"))
      return "json";
    if (e === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (t) => e.startsWith(t)
    ))
      return "blob";
    if (e.startsWith("text/"))
      return "text";
  }
}, ae = (r, e) => e ? !!(r.headers.has(e) || r.query?.[e] || r.headers.get("Cookie")?.includes(`${e}=`)) : !1, ne = async ({
  security: r,
  ...e
}) => {
  for (const t of r) {
    if (ae(e, t.name))
      continue;
    const a = await re(t, e.auth);
    if (!a)
      continue;
    const n = t.name ?? "Authorization";
    switch (t.in) {
      case "query":
        e.query || (e.query = {}), e.query[n] = a;
        break;
      case "cookie":
        e.headers.append("Cookie", `${n}=${a}`);
        break;
      default:
        e.headers.set(n, a);
        break;
    }
  }
}, P = (r) => ee({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : R(r.querySerializer),
  url: r.url
}), U = (r, e) => {
  const t = { ...r, ...e };
  return t.baseUrl?.endsWith("/") && (t.baseUrl = t.baseUrl.substring(0, t.baseUrl.length - 1)), t.headers = V(r.headers, e.headers), t;
}, ie = (r) => {
  const e = [];
  return r.forEach((t, a) => {
    e.push([a, t]);
  }), e;
}, V = (...r) => {
  const e = new Headers();
  for (const t of r) {
    if (!t)
      continue;
    const a = t instanceof Headers ? ie(t) : Object.entries(t);
    for (const [n, f] of a)
      if (f === null)
        e.delete(n);
      else if (Array.isArray(f))
        for (const c of f)
          e.append(n, c);
      else f !== void 0 && e.set(
        n,
        typeof f == "object" ? JSON.stringify(f) : f
      );
  }
  return e;
};
class T {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(e) {
    const t = this.getInterceptorIndex(e);
    this.fns[t] && (this.fns[t] = null);
  }
  exists(e) {
    const t = this.getInterceptorIndex(e);
    return !!this.fns[t];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
  }
  update(e, t) {
    const a = this.getInterceptorIndex(e);
    return this.fns[a] ? (this.fns[a] = t, e) : !1;
  }
  use(e) {
    return this.fns.push(e), this.fns.length - 1;
  }
}
const oe = () => ({
  error: new T(),
  request: new T(),
  response: new T()
}), ce = R({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), fe = {
  "Content-Type": "application/json"
}, v = (r = {}) => ({
  ...G,
  headers: fe,
  parseAs: "auto",
  querySerializer: ce,
  ...r
}), le = (r = {}) => {
  let e = U(v(), r);
  const t = () => ({ ...e }), a = (d) => (e = U(e, d), t()), n = oe(), f = async (d) => {
    const s = {
      ...e,
      ...d,
      fetch: d.fetch ?? e.fetch ?? globalThis.fetch,
      headers: V(e.headers, d.headers),
      serializedBody: void 0
    };
    s.security && await ne({
      ...s,
      security: s.security
    }), s.requestValidator && await s.requestValidator(s), s.body !== void 0 && s.bodySerializer && (s.serializedBody = s.bodySerializer(s.body)), (s.body === void 0 || s.serializedBody === "") && s.headers.delete("Content-Type");
    const u = P(s);
    return { opts: s, url: u };
  }, c = async (d) => {
    const { opts: s, url: u } = await f(d), E = {
      redirect: "follow",
      ...s,
      body: te(s)
    };
    let w = new Request(u, E);
    for (const h of n.request.fns)
      h && (w = await h(w, s));
    const O = s.fetch;
    let l = await O(w);
    for (const h of n.response.fns)
      h && (l = await h(l, w, s));
    const y = {
      request: w,
      response: l
    };
    if (l.ok) {
      const h = (s.parseAs === "auto" ? se(l.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (l.status === 204 || l.headers.get("Content-Length") === "0") {
        let g;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "text":
            g = await l[h]();
            break;
          case "formData":
            g = new FormData();
            break;
          case "stream":
            g = l.body;
            break;
          default:
            g = {};
            break;
        }
        return s.responseStyle === "data" ? g : {
          data: g,
          ...y
        };
      }
      let p;
      switch (h) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          p = await l[h]();
          break;
        case "stream":
          return s.responseStyle === "data" ? l.body : {
            data: l.body,
            ...y
          };
      }
      return h === "json" && (s.responseValidator && await s.responseValidator(p), s.responseTransformer && (p = await s.responseTransformer(p))), s.responseStyle === "data" ? p : {
        data: p,
        ...y
      };
    }
    const S = await l.text();
    let x;
    try {
      x = JSON.parse(S);
    } catch {
    }
    const j = x ?? S;
    let m = j;
    for (const h of n.error.fns)
      h && (m = await h(j, l, w, s));
    if (m = m || {}, s.throwOnError)
      throw m;
    return s.responseStyle === "data" ? void 0 : {
      error: m,
      ...y
    };
  }, i = (d) => (s) => c({ ...s, method: d }), o = (d) => async (s) => {
    const { opts: u, url: E } = await f(s);
    return M({
      ...u,
      body: u.body,
      headers: u.headers,
      method: d,
      onRequest: async (w, O) => {
        let l = new Request(w, O);
        for (const y of n.request.fns)
          y && (l = await y(l, u));
        return l;
      },
      url: E
    });
  };
  return {
    buildUrl: P,
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: t,
    head: i("HEAD"),
    interceptors: n,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: c,
    setConfig: a,
    sse: {
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: i("TRACE")
  };
}, ue = le(v({
  baseUrl: "https://localhost:44396"
}));
export {
  ue as c,
  de as f
};
//# sourceMappingURL=client.gen-miud6JxX.js.map
