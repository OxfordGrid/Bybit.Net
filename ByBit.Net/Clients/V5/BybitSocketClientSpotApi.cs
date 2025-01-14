﻿using Bybit.Net.Enums;
using Bybit.Net.Interfaces.Clients.V5;
using Bybit.Net.Objects;
using Bybit.Net.Objects.Internal.Socket;
using Bybit.Net.Objects.Models.V5;
using Bybit.Net.Objects.Options;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bybit.Net.Clients.V5
{
    /// <inheritdoc cref="IBybitSocketClientSpotApi" />
    public class BybitSocketClientSpotApi : BybitSocketClientBaseApi, IBybitSocketClientSpotApi
    {
        internal BybitSocketClientSpotApi(ILogger logger, BybitSocketOptions options)
            : base(logger, options, "/v5/public/spot")
        {
            SendPeriodic("Ping", options.V5Options.PingInterval, (connection) =>
            {
                return new BybitV5RequestMessage("ping", Array.Empty<object>(), NextId().ToString());
            });
            AddGenericHandler("Heartbeat", (evnt) => { });
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<BybitSpotTickerUpdate>> handler, CancellationToken ct = default)
            => SubscribeToTickerUpdatesAsync(new string[] { symbol }, handler, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BybitSpotTickerUpdate>> handler, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var internalData = data.Data["data"];
                if (internalData == null)
                    return;

                var desResult = Deserialize<BybitSpotTickerUpdate>(internalData);
                if (!desResult)
                {
                    _logger.Log(LogLevel.Warning, $"Failed to deserialize {nameof(BybitSpotTickerUpdate)} object: " + desResult.Error);
                    return;
                }

                handler(data.As(desResult.Data, data.Data["topic"]!.ToString().Split('.').Last()));
            });

            return await SubscribeAsync(
                BaseAddress + _baseEndpoint,
                new BybitV5RequestMessage("subscribe", symbols.Select(s => $"tickers.{s}").ToArray(), NextId().ToString()),
                null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<BybitKlineUpdate>>> handler, CancellationToken ct = default)
            => SubscribeToLeveragedTokenKlineUpdatesAsync(new string[] { symbol }, interval, handler, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenKlineUpdatesAsync(IEnumerable<string> symbols, KlineInterval interval, Action<DataEvent<IEnumerable<BybitKlineUpdate>>> handler, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var internalData = data.Data["data"];
                if (internalData == null)
                    return;

                var desResult = Deserialize<IEnumerable<BybitKlineUpdate>>(internalData);
                if (!desResult)
                {
                    _logger.Log(LogLevel.Warning, $"Failed to deserialize {nameof(BybitKlineUpdate)} object: " + desResult.Error);
                    return;
                }

                handler(data.As(desResult.Data, data.Data["topic"]!.ToString().Split('.').Last()));
            });

            return await SubscribeAsync(
                BaseAddress + _baseEndpoint,
                new BybitV5RequestMessage("subscribe", symbols.Select(s => $"kline_lt.{EnumConverter.GetString(interval)}.{s}").ToArray(), NextId().ToString()),
                null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenTickerUpdatesAsync(string symbol, Action<DataEvent<BybitLeveragedTokenTicker>> handler, CancellationToken ct = default)
            => SubscribeToLeveragedTokenTickerUpdatesAsync(new string[] { symbol }, handler, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BybitLeveragedTokenTicker>> handler, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var internalData = data.Data["data"];
                if (internalData == null)
                    return;

                var desResult = Deserialize<BybitLeveragedTokenTicker>(internalData);
                if (!desResult)
                {
                    _logger.Log(LogLevel.Warning, $"Failed to deserialize {nameof(BybitLeveragedTokenTicker)} object: " + desResult.Error);
                    return;
                }

                handler(data.As(desResult.Data, data.Data["topic"]!.ToString().Split('.').Last()));
            });

            return await SubscribeAsync(
                 BaseAddress + _baseEndpoint,
                new BybitV5RequestMessage("subscribe", symbols.Select(s => $"tickers_lt.{s}").ToArray(), NextId().ToString()),
                null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenNavUpdatesAsync(string symbol, Action<DataEvent<BybitLeveragedTokenNav>> handler, CancellationToken ct = default)
            => SubscribeToLeveragedTokenNavUpdatesAsync(new string[] { symbol }, handler, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToLeveragedTokenNavUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<BybitLeveragedTokenNav>> handler, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken>>(data =>
            {
                var internalData = data.Data["data"];
                if (internalData == null)
                    return;

                var desResult = Deserialize<BybitLeveragedTokenNav>(internalData);
                if (!desResult)
                {
                    _logger.Log(LogLevel.Warning, $"Failed to deserialize {nameof(BybitLeveragedTokenNav)} object: " + desResult.Error);
                    return;
                }

                handler(data.As(desResult.Data, data.Data["topic"]!.ToString().Split('.').Last()));
            });

            return await SubscribeAsync(
                 BaseAddress + _baseEndpoint,
                new BybitV5RequestMessage("subscribe", symbols.Select(s => $"lt.{s}").ToArray(), NextId().ToString()),
                null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection socketConnection)
        {
            if (socketConnection.ApiClient.AuthenticationProvider == null)
                return new CallResult<bool>(new NoApiCredentialsError());

            var expireTime = DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow.AddSeconds(30))!;
            var key = ((BybitAuthenticationProvider)socketConnection.ApiClient.AuthenticationProvider).GetApiKey();
            var sign = socketConnection.ApiClient.AuthenticationProvider.Sign($"GET/realtime{expireTime}");

            var authRequest = new BybitRequestMessage()
            {
                Operation = "auth",
                Parameters = new object[]
                {
                    key,
                    expireTime,
                    sign
                }
            };

            var result = false;
            var error = "unspecified error";
            await socketConnection.SendAndWaitAsync(authRequest, ClientOptions.RequestTimeout, null, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var operation = data["op"]?.ToString();
                if (operation != "auth")
                    return false;

                result = data["success"]?.Value<bool>() == true;
                error = data["ret_msg"]?.ToString();
                return true;

            }).ConfigureAwait(false);
            return result ? new CallResult<bool>(result) : new CallResult<bool>(new ServerError(error));
        }

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials) => new BybitAuthenticationProvider(credentials);
        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection socketConnection, object request, JToken data, out CallResult<T> callResult) => throw new NotImplementedException();
        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection socketConnection, SocketSubscription subscription, object request, JToken data, out CallResult<object>? callResult)
        {
            callResult = null;
            if (data.Type != JTokenType.Object)
                return false;

            var messageRequestId = data["req_id"]?.ToString();
            if (messageRequestId != null)
            {
                // Matched based on request id
                var id = ((BybitV5RequestMessage)request).RequestId;
                if (id != messageRequestId)
                    return false;

                var success1 = data["success"]?.Value<bool>() == true;
                if (success1)
                    callResult = new CallResult<object>(true);
                else
                    callResult = new CallResult<object>(new ServerError(data["ret_msg"]!.ToString()));
                return true;
            }

            // No request id
            var success = data["success"]?.Value<bool>();
            if (success == null)
                return false;

            callResult = success == true ? new CallResult<object>(true) : new CallResult<object>(new ServerError(data.ToString()));
            return true;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            if (message.Type != JTokenType.Object)
                return false;

            var topic = message["topic"]?.ToString();
            if (topic == null)
                return false;

            var requestParams = ((BybitV5RequestMessage)request).Parameters;
            if (requestParams.Any(p => topic == p.ToString()))
                return true;

            if (topic.Contains('.'))
            {
                // Some subscriptions have topics like orderbook.ETHUSDT
                // Split on `.` to get the topic and symbol
                var split = topic.Split('.');
                var symbol = split.Last();
                if (symbol.Length == 0)
                    return false;

                var mainTopic = topic.Substring(0, topic.Length - symbol.Length - 1);
                if (requestParams.Any(p => (string)p == mainTopic + ".*"))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            if (identifier == "Heartbeat")
            {
                if (message.Type != JTokenType.Object)
                    return false;

                var ret = message["ret_msg"] ?? message["op"];
                if (ret == null)
                    return false;

                var isPing = ret.ToString() == "pong";
                if (!isPing)
                    return false;

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscriptionToUnsub)
        {
            var requestParams = ((BybitV5RequestMessage)subscriptionToUnsub.Request!).Parameters;
            var message = new BybitV5RequestMessage("unsubscribe", requestParams, NextId().ToString());

            var result = false;
            await connection.SendAndWaitAsync(message, ClientOptions.RequestTimeout, null, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var messageRequestId = data["req_id"]?.ToString();
                if (messageRequestId != null)
                {
                    // Matched based on request id
                    var id = message.RequestId;
                    if (id != messageRequestId)
                        return false;

                    return true;
                }

                // No request id
                var success = data["success"]?.Value<bool>();
                if (success == null)
                    return false;

                return true;
            }).ConfigureAwait(false);
            return result;
        }
    }
}
