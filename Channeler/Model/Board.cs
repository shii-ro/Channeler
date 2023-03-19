using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Channeler.Model
{
    public class Board
    {
        public string board { get; set; }
        public string title { get; set; }
        public string description => $"/{board}/ - {title}";
        public int ws_board { get; set; }
        public int per_page { get; set; }
        public int pages { get; set; }
        public int max_filesize { get; set; }
        public int max_webm_filesize { get; set; }
        public int max_comment_chars { get; set; }
        public int max_webm_duration { get; set; }
        public int bump_limit { get; set; }
        public int image_limit { get; set; }
        public class Cooldowns {
            public int threads { get; set; }
            public int replies { get; set; }
            public int images { get; set; }
        }public Cooldowns cooldowns { get; set; }
        public string meta_description { get; set; }
        public int is_archived { get; set; }
        public int? spoilers { get; set; }
        public int? custom_spoilers { get; set; }
        public int? user_ids { get; set; }
        public int? country_flags { get; set; }
        public int? code_tags { get; set; }
        public int? webm_audio { get; set; }
        public int? min_image_width { get; set; }
        public int? min_image_height { get; set; }
        public int? oekaki { get; set; }
        public int? sjis_tags { get; set; }
        public BoardFlags board_flags { get; set; }
        public int? text_only { get; set; }
        public int? require_subject { get; set; }
    }

    public class BoardFlags
    {
        [JsonProperty("4CC")]
        public string _4CC { get; set; }
        public string ADA { get; set; }
        public string AN { get; set; }
        public string ANF { get; set; }
        public string APB { get; set; }
        public string AJ { get; set; }
        public string AB { get; set; }
        public string AU { get; set; }
        public string BB { get; set; }
        public string BM { get; set; }
        public string BP { get; set; }
        public string BS { get; set; }
        public string CL { get; set; }
        public string CO { get; set; }
        public string CG { get; set; }
        public string CHE { get; set; }
        public string CB { get; set; }
        public string DAY { get; set; }
        public string DD { get; set; }
        public string DER { get; set; }
        public string DT { get; set; }
        public string DIS { get; set; }
        public string EQA { get; set; }
        public string EQF { get; set; }
        public string EQP { get; set; }
        public string EQR { get; set; }
        public string EQT { get; set; }
        public string EQI { get; set; }
        public string EQS { get; set; }
        public string ERA { get; set; }
        public string FAU { get; set; }
        public string FLE { get; set; }
        public string FL { get; set; }
        public string GI { get; set; }
        public string HT { get; set; }
        public string IZ { get; set; }
        public string LI { get; set; }
        public string LT { get; set; }
        public string LY { get; set; }
        public string MA { get; set; }
        public string MAU { get; set; }
        public string MIN { get; set; }
        public string NI { get; set; }
        public string NUR { get; set; }
        public string OCT { get; set; }
        public string PAR { get; set; }
        public string PC { get; set; }
        public string PCE { get; set; }
        public string PI { get; set; }
        public string PLU { get; set; }
        public string PM { get; set; }
        public string PP { get; set; }
        public string QC { get; set; }
        public string RAR { get; set; }
        public string RD { get; set; }
        public string RLU { get; set; }
        public string S1L { get; set; }
        public string SCO { get; set; }
        public string SHI { get; set; }
        public string SIL { get; set; }
        public string SON { get; set; }
        public string SP { get; set; }
        public string SPI { get; set; }
        public string SS { get; set; }
        public string STA { get; set; }
        public string STL { get; set; }
        public string SPT { get; set; }
        public string SUN { get; set; }
        public string SUS { get; set; }
        public string SWB { get; set; }
        public string TFA { get; set; }
        public string TFO { get; set; }
        public string TFP { get; set; }
        public string TFS { get; set; }
        public string TFT { get; set; }
        public string TFV { get; set; }
        public string TP { get; set; }
        public string TS { get; set; }
        public string TWI { get; set; }
        public string TX { get; set; }
        public string VS { get; set; }
        public string ZE { get; set; }
        public string ZS { get; set; }
        public string AC { get; set; }
        public string BL { get; set; }
        public string CF { get; set; }
        public string CM { get; set; }
        public string CT { get; set; }
        public string DM { get; set; }
        public string EU { get; set; }
        public string FC { get; set; }
        public string GN { get; set; }
        public string GY { get; set; }
        public string JH { get; set; }
        public string KN { get; set; }
        public string MF { get; set; }
        public string NB { get; set; }
        public string NT { get; set; }
        public string NZ { get; set; }
        public string PR { get; set; }
        public string RE { get; set; }
        public string MZ { get; set; }
        public string TM { get; set; }
        public string TR { get; set; }
        public string UN { get; set; }
        public string WP { get; set; }
    }

    public class BoardList
    {
        public List<Board> boards { get; set; }
    }
}
