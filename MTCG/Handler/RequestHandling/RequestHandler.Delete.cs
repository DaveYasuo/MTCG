using ServerModule.Mapping;
using ServerModule.Responses;

namespace MTCG.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response DeleteTradings(RequestData requestData)
        {
            var username = requestData.Authentication.Username;
            var storeId = requestData.PathVariable;
            if (username is null || storeId is null) return Response.Status(Status.BadRequest);
            var dealCard = DataHandler.GetCardOfStore(storeId);
            if (dealCard == null) return Response.Status(Status.BadRequest);
            if (DataHandler.GetCard(dealCard.CardToTrade).Username != username)
                return Response.Status(Status.Forbidden);
            return DataHandler.DeleteTradingDeal(storeId)
                ? Response.PlainText("Trade deleted successfully")
                : Response.Status(Status.InternalServerError);
        }
    }
}