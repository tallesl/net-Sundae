<p align="center">
  <a href="#">
    <img alt="logo" src="Sundae.png">
  </a>
</p>

# Sundae

This is a project started out of need, on a Sunday afternoon. It grew from the frustration of the available .NET XMPP libraries, due to them being closed source, unmaintained, hard to debug, or a mix of all of those.

Sundae aims to be well-written and production-ready first, and RFC complaint second. [RFC 6120](https://tools.ietf.org/html/rfc6120) and [RFC 6121](https://tools.ietf.org/html/rfc6121) with the addition of [XEP-0054](https://xmpp.org/extensions/xep-0054.html) (vCard), [XEP-0065](https://xmpp.org/extensions/xep-0065.html) (SOCKS5 file transfer), [XEP-0077](https://xmpp.org/extensions/xep-0077.html) (registration) and [XEP-0078](https://xmpp.org/extensions/xep-0078.html) (non-SASL authentication) are the priority.

## ejabberd

[ejabberd](https://www.ejabberd.im) is the XMPP server used for tests. [Manual tests](TestApplication/Program.cs), unfortunately, there's no automated tests at the moment.

SASL is not implemented, the `Authenticate` and `Register` methods are predicated on ejabberd behavior. `mod_legacy_auth` must be enabled due to the lack of TLS support right now. Sorry.
