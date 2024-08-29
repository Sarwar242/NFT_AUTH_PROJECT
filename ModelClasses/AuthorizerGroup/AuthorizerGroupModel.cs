namespace ModelClasses.AuthorizerGroup
{
    public class AuthorizerGroupModel
    {
        public int? group_id {  get; set; }
        public string? group_nm { get; set; }
        public string? designation_nm { get; set; }
        public int? authrizer_sl_no { get; set; }
        public int? desig_override_auth_flag { get; set; }
        public string? make_by {  get; set; }
        public string? last_action { get; set; }
    }
}
